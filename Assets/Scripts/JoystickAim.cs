using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickAim : MonoBehaviour
{
    [SerializeField] float Move_speed = 30f;

    Vector2 moveInput;
    // Start is called before the first frame update
    void Start()
    {

    }

    
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

}
