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

    private bool isUsingMouse = true;

    void Update()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
          isUsingMouse = false;
        }
        else
        {
           isUsingMouse= true;
        }


        if (isUsingMouse)
        {
            Vector3 cursorScreenPosition = Input.mousePosition;
            Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(cursorScreenPosition + Vector3.forward * 10f);
            transform.position = cursorWorldPosition;
        }
        else
        {
            // Check for controller input
            if (Gamepad.current != null)
            {
                Cursor.visible = false;
                movementInput = Gamepad.current.rightStick.ReadValue();
                float angle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;
                Vector3 offset = Quaternion.Euler(0, 0, angle) * Vector3.right * distanceFtomPlayer;
                transform.position = playerTransform.position + offset;
            }
            else
            {
                Cursor.visible = true;
                // If no controller is connected, fallback to mouse input
                Vector3 cursorScreenPosition = Input.mousePosition;
                Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(cursorScreenPosition + Vector3.forward * 10f);
                transform.position = cursorWorldPosition;
            }
        }



    }
}

