using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    SpriteRenderer spriteRenderer;

    public float walkSpeed = 8f;
    public float Sprint = 1f;
    public float maxSpeed = 0.2f;
    float inputHorizontal;
    float inputVertical;

   

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        if (inputHorizontal != 0 || inputVertical != 0)
        {

            // calculate walkspeed
            if (inputHorizontal != 0 && inputVertical != 0)
            {
                walkSpeed = 4.6f * Sprint;
            }
            else
            {
                walkSpeed = 6f * Sprint;
            }


            FlipSprite();

            myRigidbody.velocity = new Vector2(inputHorizontal * walkSpeed, inputVertical * walkSpeed);
        }
        else
        {
            myRigidbody.velocity = new Vector2(0f, 0f);
        }


        if (Input.GetKey(KeyCode.LeftShift))
        {
            Sprint = 1.5f;
        }
        else
        {
            Sprint = 1f;
        }


    }

    void FlipSprite()
    {
        // flip sprite x
        if (inputHorizontal < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (inputHorizontal > 0) // more than 0
        {
            spriteRenderer.flipX = false;
        }
    }

}