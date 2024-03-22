using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
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
    [SerializeField] float invinsibilityLenght = 1f;

    [NonSerialized] public Vector2 exteriorVelocity;

    private Rigidbody2D myRigidbody;
    private Animator animator;

    private float rollSpeed;
    float rollSpeedDropMultiplier = 8f;
    private State state;
    float rollResetTime;
    bool isRollDelaying = false;
    bool isDead = false;
    bool invinsibility = false;


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

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
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
        }
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    IEnumerator InvisibilityDelayRoutine()
    {
        invinsibility = true;
        yield return new WaitForSeconds(invinsibilityLenght);
        invinsibility = false;
    }

    public void SetDamageTakenFalse()
    {
        invinsibility = false;
    }



}