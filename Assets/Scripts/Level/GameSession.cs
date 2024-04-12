using System;
using UnityEngine;
using UnityEngine.Events;

public class GameSession : MonoBehaviour
{
    [Header("Load Settings")]
    public bool spawnRooms = true;
    public LevelState state = LevelState.Loading;

    [NonSerialized] public UnityEvent onEnemyKill;
    [NonSerialized] public UnityEvent OnNewState;

    public enum LevelState
    {
        Loading,
        Running,
        Paused
    }
    public MusicManager soundManager { get; private set; }
    public Camera mainCamera { get; private set; }
    public Player player;
    public Transform enemyContainer;
    public MusicManager musicManager { get; private set; }

    public bool gamePaused = false;


    private void Awake()
    {
        onEnemyKill = new UnityEvent();
        OnNewState = new UnityEvent();
    }

    private void Start()
    {
        soundManager = FindObjectOfType<MusicManager>();
        player = FindObjectOfType<Player>();
        musicManager = FindObjectOfType<MusicManager>();
        mainCamera = Camera.main;
    }

    LevelState previousState;
    private void Update()
    {
        if (state != previousState)
        {
            OnNewState.Invoke();
        }
        previousState = state;
    }


    public void SetState(LevelState state)
    {
        this.state = state;
    }
}


