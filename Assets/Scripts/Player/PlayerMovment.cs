using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour
{
    private enum State
    {
        normal,
        rolling,
    }

    [SerializeField] float Move_speed = 30f;
    [SerializeField] float rollSpeedMinimum = 50f;
    [SerializeField] float health = 100f;
    [SerializeField] float rolldelay = 0.2f;

    private Rigidbody2D MyRigidbody;
    private Vector2 movedir;
    private Vector2 rolldir;
    private float rollSpeed;
    float rollSpeedDropMultiplier = 8f;
    private State state;
    float rollResetTime;
    bool isRollDelaying = false;

    Vector2 moveInput;


    private void Awake()
    {
        state = State.normal;
        MyRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
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
        RollDelay();
    }


    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnDash()
    {
        if(isRollDelaying) { return; }
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

    public void TakeDamage(float damage)
    {
        if (state == State.rolling)
        {
            return;
        }
        health -= damage;
        if (health < 0)
        {
            Debug.Log("you died >:C");
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.normal:
                MyRigidbody.velocity = moveInput * Move_speed;

                break;
            case State.rolling:
                MyRigidbody.velocity = moveInput * rollSpeed;
                break;
        }
    }
}