using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    Rigidbody2D MyRigidbody;

public float walkSpeed = 8f;
public float Sprint = 1f;
public float maxSpeed = 0.2f;
float inputHorizontal;
float inputVertical;

private void Start()
{
    MyRigidbody = GetComponent<Rigidbody2D>();
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
        if (inputHorizontal != 0 && inputVertical != 0)
        {
                walkSpeed = 4.6f * Sprint;
        }
        else
            {
                walkSpeed = 6f * Sprint;
            }

        MyRigidbody.velocity = new Vector2(inputHorizontal * walkSpeed, inputVertical * walkSpeed);
    }
    else
    {
        MyRigidbody.velocity = new Vector2(0f, 0f);
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

}