using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private enum State
    {
        normal,
        rolling,
    }

    [SerializeField] LayerMask ignoreOnDash;
    public float Move_speed = 30f;
    //[SerializeField] float rollSpeedMinimum = 50f;
    [SerializeField] float rolldelay = 0.2f;
    [SerializeField] int health = 9;
    [SerializeField] float drag = 0.9f;
    [SerializeField] float invinsibilityLenght = 1f;
    [SerializeField] float invisibilityLengthDash = 0.2f;
    [SerializeField] GameObject BloodSplatVFX;


    [NonSerialized] public Vector2 exteriorVelocity;


    public GameObject crosshair;
    public bool isDead = false;
    public UnityEvent onInteract;

    public bool hasKey;


    private float rollSpeed;
    private float rollSpeedDropMultiplier = 8f;
    private State state;
    private float rollResetTime;
    private bool isRollDelaying = false;
    private float Crosshair_distance = 30f;
    
    private bool invinsibility = false;


    private Vector2 moveInput;
    private Vector2 AimDirection;

    private ContactFilter2D noFilter;

    // refs
    private Rigidbody2D myRigidbody;
    private Animator animator;
    private Collider2D myCollider;
    private GameSession gameSession;
    private UserInput userInput;

    private CatEchoSpawner dashingEchoSpawner;

    private SceneLoader loader;



    private void Awake()
    {
        onInteract = new UnityEvent();
    }

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        myCollider = GetComponent<Collider2D>();
        gameSession = GameSession.Instance;
        dashingEchoSpawner = GetComponentInChildren<CatEchoSpawner>();

        FetchExternalRefs();

        // Input Events
        userInput.onMove.AddListener(OnMove);
        userInput.onDash.AddListener(OnDash);

        // Create a contact filter that includes triggers
        noFilter = new ContactFilter2D();
        noFilter.useTriggers = true;


        state = State.normal;
        rollResetTime = rolldelay;
        

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void FetchExternalRefs()
    {
        loader = FindObjectOfType<SceneLoader>();
        userInput = FindObjectOfType<UserInput>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FetchExternalRefs();
/*        userInput.onMove.AddListener(OnMove);
        userInput.onAiming.AddListener(OnAim);
        userInput.onDash.AddListener(OnDash);*/
        //transform.position = Vector3.zero;
    }


    private void Update()
    {
        if (isDead || GameSession.state != GameSession.GameState.Running)
        {
            myRigidbody.velocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
            return;
        }
        if (state == State.rolling)
        {
            int dashLayer = LayerMask.NameToLayer("PlayerDash");
            int playerLayer = LayerMask.NameToLayer("Player");
            rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;
            gameObject.layer = dashLayer;

            if (rollSpeed < Move_speed)
            {
                animator.SetTrigger("DashInvisibility");
                StartCoroutine(InvisibilityDelayRoutine(invisibilityLengthDash));
                gameObject.layer = playerLayer;
                state = State.normal;
                dashingEchoSpawner.StopDashTrail();
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
        if (GameSession.state != GameSession.GameState.Running)
        {
            moveInput = Vector2.zero;
            return;
        }
        moveInput = moveVector;
        
    }

    void OnDash()
    {
        if (myRigidbody.velocity == Vector2.zero || isRollDelaying || GameSession.state != GameSession.GameState.Running) { return; }

        dashingEchoSpawner.StrartingDashTrail(transform.rotation);
        //animator.SetTrigger("Dash");
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
        StartCoroutine(InvisibilityDelayRoutine(invinsibilityLenght));
        SpawnBloodSplatVFX();
        if (health <= 0)
        {
            isDead = true;
            animator.SetTrigger("IsDead");
            StartCoroutine(WaitForDeathAnimation());
        }
    }

    IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(2f);
        gameSession.deathScreen.gameObject.SetActive(true);
    }

    private void SpawnBloodSplatVFX()
    {
        Instantiate(BloodSplatVFX, transform.position, Quaternion.identity);
    }

    public void AddHealth(int hp)
    {
        health += hp;
        health = Mathf.Clamp(health, 0, 9);
        animator.SetTrigger("WasHealed");
    }

    void BackToMenu()
    {
        loader.LoadMainMenu();
    }

    IEnumerator InvisibilityDelayRoutine(float invinsibilityLength)
    {
        invinsibility = true;
        yield return new WaitForSeconds(invinsibilityLength);
        invinsibility = false;
    }

    public int GetHealth()
    {
        return health;
    }

    public bool IsOverlapping<T>(GameObject toCompare) where T : Component
    {
        // Get colliders
        Collider2D[] colliders = new Collider2D[10];
        Physics2D.OverlapCollider(myCollider, noFilter, colliders);

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

}