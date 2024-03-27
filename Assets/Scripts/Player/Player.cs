using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private enum State
    {
        normal,
        rolling,
    }

    [SerializeField] float Move_speed = 30f;
    [SerializeField] float rollSpeedMinimum = 50f;
    [SerializeField] float rolldelay = 0.2f;
    [SerializeField] int health = 9;

    [SerializeField] float drag = 0.9f;
    [SerializeField] float invinsibilityLenght = 1f;

    [NonSerialized] public Vector2 exteriorVelocity;


    public GameObject Crosshair;


    public bool isDead = false;

    private float rollSpeed;
    float rollSpeedDropMultiplier = 8f;
    private State state;
    float rollResetTime;
    bool isRollDelaying = false;
    float Crosshair_distance = 30f;
    
    bool invinsibility = false;


    Vector2 moveInput;
    Vector2 AimDirr;

    // refs
    private Rigidbody2D myRigidbody;
    private Animator animator;
    private SceneLoader loader;


    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        loader = FindObjectOfType<SceneLoader>();

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

    //void OnInteract()
    //{
    //    inventory.OnInteract();
    //}

    // on_move
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        
    }

    void OnAim(InputValue value)
    {
        AimDirr = value.Get<Vector2>();
    }

    void OnDash()
    {
        if (isRollDelaying) { return; }

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


    void Aim()
    {
        if(Crosshair == null) { return; }
        Crosshair.transform.localPosition = AimDirr * Crosshair_distance;
    }
}