using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField] int health = 100;

    [SerializeField] float drag = 0.9f;

    [NonSerialized] public Vector2 exteriorVelocity;

    private Rigidbody2D myRigidbody;
    private Animator animator;

    private Vector2 movedir;
    private Vector2 rolldir;
    private float rollSpeed;
    float rollSpeedDropMultiplier = 8f;
    private State state;
    float rollResetTime;
    bool isRollDelaying = false;
    bool isDead = false;    

    Vector2 moveInput;



    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();  

        state = State.normal;
        rollResetTime = rolldelay;
    }

    private void Update()
    {
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
    }


    void OnMove(InputValue value)
    {
        if(isDead) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnDash()
    {
        if (isDead) { return; }
        if (isRollDelaying) { return; }
        rolldir = movedir;

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
        if (state == State.rolling) { return; }
        animator.SetTrigger("WasHurt");
        health -= damage;
        if (health < 0)
        {
            isDead = true;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.normal:
                myRigidbody.velocity = moveInput.normalized * Move_speed + exteriorVelocity;

                break;
            case State.rolling:
                myRigidbody.velocity = moveInput.normalized * rollSpeed + exteriorVelocity;
                break;
        }
        exteriorVelocity *= drag;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "Enemy")
        {

        }
    }

    public bool GetIsDead()
    {
        return isDead;
    }
}