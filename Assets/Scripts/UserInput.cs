using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UserInput : MonoBehaviour
{
    [SerializeField] float fireCooldown = .1f;

    [NonSerialized] public SafeUnityEvent<Vector2> onMove;
    [NonSerialized] public SafeUnityEvent<Vector2> onAiming;
    [NonSerialized] public SafeUnityEvent<float> onScroll;
    [NonSerialized] public SafeUnityEvent onFire;
    [NonSerialized] public SafeUnityEvent onDash;
    [NonSerialized] public SafeUnityEvent onInteract;
    [NonSerialized] public SafeUnityEvent onTogglePause;
    [NonSerialized] public SafeUnityEvent onReload;
    [NonSerialized] public SafeUnityEvent onToggleConsole;

    PlayerInput playerInput;
    Crosshair crosshair;
    float lastFireTime = 0f;
    

    bool firing = false;

    private void Awake()
    {
        if (onMove != null) return;
        onMove = new SafeUnityEvent<Vector2>();
        onAiming = new SafeUnityEvent<Vector2>();
        onScroll = new SafeUnityEvent<float>();
        onFire = new SafeUnityEvent();
        onDash = new SafeUnityEvent();
        onInteract = new SafeUnityEvent();
        onTogglePause = new SafeUnityEvent();
        onReload = new SafeUnityEvent();
        onToggleConsole = new SafeUnityEvent(); 
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
            onFire.SafeInvoke();
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
        onToggleConsole.RemoveAllListeners();
    }

    void OnMove(InputValue value)
    {
        onMove.SafeInvoke(value.Get<Vector2>());
    }

    void OnAiming(InputValue value)
    {
        onAiming.SafeInvoke(value.Get<Vector2>());
    }

    void OnScroll(InputValue value)
    {
        onScroll.SafeInvoke(value.Get<float>());
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
        onDash.SafeInvoke();
    }

    void OnInteract()
    {
        onInteract.SafeInvoke();
    }

    void OnTogglePause()
    {
        onTogglePause.SafeInvoke();
    }

    void OnReload()
    {
        onReload.SafeInvoke();
    }

    void OnToggleConsole()
    {
        Debug.Log("Input ToggleConsole");
        onToggleConsole.SafeInvoke();
    }
}



public class SafeUnityEvent<T> : UnityEvent<T>
{
    private List<UnityAction<T>> listeners = new List<UnityAction<T>>();

    public new void AddListener(UnityAction<T> call)
    {
        if (call != null)
        {
            listeners.Add(call);
            base.AddListener(call);
        }
    }

    public new void RemoveListener(UnityAction<T> call)
    {
        if (call != null)
        {
            listeners.Remove(call);
            base.RemoveListener(call);
        }
    }

    public void SafeInvoke(T arg)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            var listener = listeners[i];
            if (listener.Target == null || listener.Target.Equals(null))
            {
                RemoveListener(listener);
            }
        }

        base.Invoke(arg);
    }
}

public class SafeUnityEvent : UnityEvent
{
    private List<UnityAction> listeners = new List<UnityAction>();

    public new void AddListener(UnityAction call)
    {
        if (call != null)
        {
            listeners.Add(call);
            base.AddListener(call);
        }
    }

    public new void RemoveListener(UnityAction call)
    {
        if (call != null)
        {
            listeners.Remove(call);
            base.RemoveListener(call);
        }
    }

    public void SafeInvoke()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            var listener = listeners[i];
            if (listener.Target == null || listener.Target.Equals(null))
            {
                RemoveListener(listener);
            }
        }

        base.Invoke();
    }
}



