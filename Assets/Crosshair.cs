using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Crosshair : MonoBehaviour
{
    public Transform playerTransform; // Reference to the player's Transform
    public float orbitSpeed = 5f; // Speed of orbit
    public float distanceFtomPlayer = 3f;

    private Vector2 movementInput;

    void Update()
    {


        movementInput = Gamepad.current.rightStick.ReadValue();
        float angle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;
        Vector3 offset = Quaternion.Euler(0, 0, angle) * Vector3.right * distanceFtomPlayer;
        transform.position = playerTransform.position + offset;


        if (Input.GetJoystickNames().Length > 0)
        {
            Debug.Log("Controller detected");
        }
        else
        {
            Debug.Log("Keyboard/Mouse detected");

        }

    }
}

