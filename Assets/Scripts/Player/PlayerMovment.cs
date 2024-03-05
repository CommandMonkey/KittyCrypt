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
    private Rigidbody2D MyRigidbody;
    private Vector2 movedir;
    private Vector2 rolldir;
    private float rollSpeed;
    float rollSpeedDropMultiplier = 8f;
    private State state;

    Vector2 moveInput;

    private void Awake()
    {
        state = State.normal;
        MyRigidbody = GetComponent<Rigidbody2D>();
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
        
        
    }


    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnDash()
    {
        rolldir = movedir;

        rollSpeed = 40f;
        state = State.rolling;
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