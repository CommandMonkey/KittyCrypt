using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Crosshair : MonoBehaviour
{
    public Transform playerTransform; // Reference to the player's Transform
    public float orbitSpeed = 5f; // Speed of orbit
    public float distanceFromPlayer = 3f;

    private Vector2 movementInput;

    private bool isUsingMouse = true;

    SpriteRenderer spriteRenderer;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

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
                if (Gamepad.current.rightStick.ReadValue() != Vector2.zero)
                {
                    movementInput = Gamepad.current.rightStick.ReadValue();
                }
                float angle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;
                Vector3 offset = Quaternion.Euler(0, 0, angle) * Vector3.right * distanceFromPlayer;
                transform.position = playerTransform.position + offset;
                spriteRenderer.enabled = true;
            }
            else
            {
                Vector3 cursorScreenPosition = Input.mousePosition;
                Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(cursorScreenPosition + Vector3.forward * 10f);
                transform.position = cursorWorldPosition;
                spriteRenderer.enabled = false; 
            }
        }
    }
}

