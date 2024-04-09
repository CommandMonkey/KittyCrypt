using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private enum State
    {
        normal,
        rolling,
    }

    [SerializeField] float Move_speed = 30f;
    //[SerializeField] float rollSpeedMinimum = 50f;
    [SerializeField] float rolldelay = 0.2f;
    [SerializeField] int health = 9;

    [SerializeField] float drag = 0.9f;
    [SerializeField] float invinsibilityLenght = 1f;

    [NonSerialized] public Vector2 exteriorVelocity;


    public GameObject crosshair;
    public bool isDead = false;
    public UnityEvent onInteract;

    private float rollSpeed;
    float rollSpeedDropMultiplier = 8f;
    private State state;
    float rollResetTime;
    bool isRollDelaying = false;
    float Crosshair_distance = 30f;
    
    bool invinsibility = false;


    Vector2 moveInput;
    Vector2 AimDirection;

    ContactFilter2D noFilter;

    // refs
    private Rigidbody2D myRigidbody;
    private Animator animator;
    private SceneLoader loader;
    private LevelManager levelManager;
    private UserInput userInput;
    private BoxCollider2D boxCollider;
    public GameObject reloadCircle;


    private void Awake()
    {
        onInteract = new UnityEvent();
        reloadCircle = GameObject.FindGameObjectWithTag("ReloadCircle");
        if(reloadCircle != null )
        {
            reloadCircle.SetActive(false);
        }
    }

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        loader = FindObjectOfType<SceneLoader>();
        levelManager = FindObjectOfType<LevelManager>();
        userInput = FindObjectOfType<UserInput>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Input Events
        userInput.onMove.AddListener(OnMove);
        userInput.onAiming.AddListener(OnAim);
        userInput.onDash.AddListener(OnDash);

        // Create a contact filter that includes triggers
        noFilter = new ContactFilter2D();
        noFilter.useTriggers = true;


        state = State.normal;
        rollResetTime = rolldelay;

    }

    private void Update()
    {
        if (isDead)
        {
            myRigidbody.velocity = Vector2.zero;
            return;
        }
        if (state == State.rolling) { 
            // dash/roll range
            
            rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;


            if (rollSpeed < Move_speed)
            {
                state = State.normal;
            }
        }

        if (moveInput != Vector2.zero)
        {
            animator.SetBool("IsWalking", true);
        }else
        {
            animator.SetBool("IsWalking", false);
        }
        RollDelay();
        Aim();
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            myRigidbody.velocity = Vector2.zero;
            return;
        }
        switch (state)
        {
            case State.normal:
                myRigidbody.velocity = moveInput.normalized * Move_speed + exteriorVelocity;

                break;
            case State.rolling:
                myRigidbody.velocity = moveInput.normalized * rollSpeed + exteriorVelocity;
                break;
        }

        Flip();

        exteriorVelocity *= drag;
    }

    // flip 
    private void Flip()
    {
        if (moveInput.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (moveInput.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public void OnMove(Vector2 moveVector)
    {
        if (levelManager.state != LevelManager.LevelState.Running) return;
        moveInput = moveVector;
        
    }

    void OnAim(Vector2 aimDirection)
    {
        AimDirection = aimDirection;
;
    }

    void OnDash()
    {
        if (isRollDelaying || levelManager.state != LevelManager.LevelState.Running) { return; }

        rollSpeed = 50f;
        state = State.rolling;
        isRollDelaying = true;
    }

    private void RollDelay()
    {
        if (!isRollDelaying) { return; }
        rollResetTime -= Time.deltaTime;

        if(rollResetTime <= 0 )
        {
            isRollDelaying = false;
            rollResetTime = rolldelay;
        }
    }

    public void TakeDamage(int damage)
    {
        if (invinsibility || state == State.rolling) { return; }
        animator.SetTrigger("WasHurt");
        health -= damage;
        StartCoroutine(InvisibilityDelayRoutine());
        if (health <= 0)
        {
            isDead = true;
            animator.SetTrigger("IsDead");
            Invoke("BackToMenu", 1f);
        }
    }

    void BackToMenu()
    {
        loader.LoadMainMenu();
    }

    IEnumerator InvisibilityDelayRoutine()
    {
        invinsibility = true;
        yield return new WaitForSeconds(invinsibilityLenght);
        invinsibility = false;
    }

    public int GetHealth()
    {
        return health;
    }

    public bool IsOverlapping<T>(GameObject toCompare) where T : Component
    {
        Debug.Log("AAAAAA----: " + toCompare.name + "parent: " + toCompare.transform.parent.name);  
        // Get colliders
        Collider2D[] colliders = new Collider2D[10];
        Physics2D.OverlapCollider(boxCollider, noFilter, colliders);

        // Find Room or entrance Colliders
        foreach (Collider2D c in colliders)
        {
            if (c != null)
            {
                if (c.gameObject == toCompare)
                {
                    return true;
                }
            }
        }

        return false; // No matching component found
    }


    public Vector3 PredictFuturePosition(float timeInFuture)
    {
        // Calculate future position based on current velocity and time
        Vector3 futurePosition = myRigidbody.position + myRigidbody.velocity * timeInFuture;

        return futurePosition;
    }



    void Aim()
    {
        if(crosshair == null) { return; }
        crosshair.transform.localPosition = AimDirection * Crosshair_distance;
    }
}