using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UserInput : MonoBehaviour
{
    public UnityEvent<Vector2> onMove;
    public UnityEvent<Vector2> onAiming;
    public UnityEvent<float> onScroll;
    public UnityEvent onFire;
    public UnityEvent onDash;
    public UnityEvent onInteract;
    public UnityEvent onTogglePause;

    private void Awake()
    {
        onMove = new UnityEvent<Vector2>();
        onAiming = new UnityEvent<Vector2>();
        onScroll = new UnityEvent<float>();
        onFire = new UnityEvent();
        onDash = new UnityEvent();
        onInteract = new UnityEvent();
        onTogglePause = new UnityEvent();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        onMove.RemoveAllListeners();
        onAiming.RemoveAllListeners();
        onScroll.RemoveAllListeners();
        onFire.RemoveAllListeners();
        onDash.RemoveAllListeners();
        onInteract.RemoveAllListeners();
    }

    public void OnMove(InputValue value)
    {
        onMove.Invoke(value.Get<Vector2>());
    }

    public void OnAiming(InputValue value)
    {
        onAiming.Invoke(value.Get<Vector2>());
    }

    public void OnScroll(InputValue value)
    {
        onScroll.Invoke(value.Get<float>());
    }

    public void OnFire()
    {
        onFire.Invoke();
    }

    public void OnDash()
    {
        onDash.Invoke();
    }

    public void OnInteract()
    {
        onInteract.Invoke();
    }

    void OnTogglePause()
    {
        onTogglePause.Invoke();
    }
}
