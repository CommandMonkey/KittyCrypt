using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [Header("Load Settings")]
    public bool spawnRooms = true;
    public GameState state = GameState.Loading;

    [NonSerialized] public UnityEvent onEnemyKill;
    [NonSerialized] public UnityEvent OnNewState;

    public enum GameState
    {
        Loading,
        Running,
        Paused
    }
    public Camera mainCamera { get; private set; }
    public Player player;
    public Transform enemyContainer;
    public MusicManager musicManager { get; private set; }

    
    private void Awake()
    {
        GameSession[] gameSessions = FindObjectsByType<GameSession>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (gameSessions.Length > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        onEnemyKill = new UnityEvent();
        OnNewState = new UnityEvent();
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        musicManager = FindObjectOfType<MusicManager>();
        mainCamera = Camera.main;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        musicManager = FindObjectOfType<MusicManager>();
    }

    GameState previousState;
    private void Update()
    {
        if (state != previousState)
        {
            OnNewState.Invoke();
        }
        previousState = state;
    }


    public void SetState(GameState state)
    {
        this.state = state;
    }
}


