using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UserInput : MonoBehaviour
{
    [SerializeField] float fireCooldown = .1f;

    [NonSerialized] public UnityEvent<Vector2> onMove;
    [NonSerialized] public UnityEvent<Vector2> onAiming;
    [NonSerialized] public UnityEvent<float> onScroll;
    [NonSerialized] public UnityEvent onFireEvent;
    [NonSerialized] public UnityEvent onDash;
    [NonSerialized] public UnityEvent onInteract;
    [NonSerialized] public UnityEvent onTogglePause;
    [NonSerialized] public UnityEvent onReload;

    PlayerInput playerInput;
    float lastFireTime = 0f;


    bool firing = false;

    private void Awake()
    {
        Debug.Log("UserInput Awake");
        onMove = new UnityEvent<Vector2>();
        onAiming = new UnityEvent<Vector2>();
        onScroll = new UnityEvent<float>();
        onFireEvent = new UnityEvent();
        onDash = new UnityEvent();
        onInteract = new UnityEvent();
        onTogglePause = new UnityEvent();
        onReload = new UnityEvent();

        ClearListeners();   
    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        //Debug.Log("delta: " + (Time.time - lastFireTime));
        var fire = playerInput.actions["Fire"];
        if (Time.time - lastFireTime > fireCooldown && fire.IsPressed())
        {
            onFireEvent.Invoke();
            lastFireTime = Time.time;
        }
    }

    void ClearListeners()
    {
        onMove.RemoveAllListeners();
        onAiming.RemoveAllListeners();
        onScroll.RemoveAllListeners();
        onFireEvent.RemoveAllListeners();
        onDash.RemoveAllListeners();
        onInteract.RemoveAllListeners();
        onReload.RemoveAllListeners();
    }

    void OnMove(InputValue value)
    {
        onMove.Invoke(value.Get<Vector2>());
    }

    void OnAiming(InputValue value)
    {
        onAiming.Invoke(value.Get<Vector2>());
    }

    void OnScroll(InputValue value)
    {
        onScroll.Invoke(value.Get<float>());
    }

/*
    void OnFire()
    {
        if (Time.time - lastFireTime > fireCooldown)
        {
            onFireEvent.Invoke();
            lastFireTime = Time.time;
        }
    }
*/

    void OnDash()
    {
        onDash.Invoke();
    }

    void OnInteract()
    {
        Debug.Log("Interact!!!");
        onInteract.Invoke();
    }

    void OnTogglePause()
    {
        onTogglePause.Invoke();
    }

    void OnReload()
    {
        onReload.Invoke();
    }

}
