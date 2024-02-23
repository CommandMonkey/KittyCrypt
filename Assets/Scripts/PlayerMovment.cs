using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

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
    private State state;


    private void Awake()
    {
        state = State.normal;
        MyRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        switch (state)
        {
            case State.normal:

                //float moveX = 0f;
                //float moveY = 0f;

                float moveY = Input.GetAxisRaw("Vertical");

                float moveX = Input.GetAxisRaw("Horizontal");
               

                movedir = new Vector2(moveX, moveY).normalized;

                if (Input.GetButtonDown("Jump"))
                {
                    rolldir = movedir;
                    // ändra dash/roll här
                    rollSpeed = 40f;
                    state = State.rolling;
                }
                break;
            case State.rolling:
                // dash/roll range
                float rollSpeedDropMunlitplier = 8f;
                rollSpeed -= rollSpeed * rollSpeedDropMunlitplier * Time.deltaTime;

                
                if (rollSpeed < rollSpeedMinimum)
                {
                    state = State.normal;
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.normal:


                MyRigidbody.velocity = movedir * Move_speed;


                break;
            case State.rolling:
                MyRigidbody.velocity = rolldir * rollSpeed;
                break;
        }
    }
}