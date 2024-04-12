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

    UserInputActions userInput;
    float lastFireTime = 0f;

    bool firing = false;

    private void Awake()
    {
        onMove = new UnityEvent<Vector2>();
        onAiming = new UnityEvent<Vector2>();
        onScroll = new UnityEvent<float>();
        onFireEvent = new UnityEvent();
        onDash = new UnityEvent();
        onInteract = new UnityEvent();
        onTogglePause = new UnityEvent();
        onReload = new UnityEvent();

        userInput = new UserInputActions();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;



    }

    private void Update()
    {
        //Debug.Log("delta: " + (Time.time - lastFireTime));
        if (Time.time - lastFireTime > fireCooldown && Input.GetMouseButton(0))
        {
            Debug.Log("YE BOIIIIII");
            onFireEvent.Invoke();
            lastFireTime = Time.time;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
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
