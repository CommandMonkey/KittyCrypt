using System;
using System.Runtime.CompilerServices;
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
    [NonSerialized] public UnityEvent onFire;
    [NonSerialized] public UnityEvent onDash;
    [NonSerialized] public UnityEvent onInteract;
    [NonSerialized] public UnityEvent onTogglePause;
    [NonSerialized] public UnityEvent onReload;

    PlayerInput playerInput;
    Crosshair crosshair;
    float lastFireTime = 0f;


    bool firing = false;

    private void Awake()
    {
        onMove = new UnityEvent<Vector2>();
        onAiming = new UnityEvent<Vector2>();
        onScroll = new UnityEvent<float>();
        onFire = new UnityEvent();
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
        var fire = playerInput.actions["Fire"];
        if (Time.time - lastFireTime > fireCooldown && fire.IsPressed())
        {
            onFire.Invoke();
            lastFireTime = Time.time;
        }
    }

    void ClearListeners()
    {
        onMove.RemoveAllListeners();
        onAiming.RemoveAllListeners();
        onScroll.RemoveAllListeners();
        onFire.RemoveAllListeners();
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
