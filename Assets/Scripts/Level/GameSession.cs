using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [Header("Load Settings")]
    public bool spawnRooms = true;
    public static GameState state = GameState.Loading;

    [NonSerialized] public UnityEvent onEnemyKill;
    [NonSerialized] public UnityEvent OnNewState;

    public enum GameState
    {
        Loading,
        Running,
        Paused
    }
    public int levelIndex { get; private set; } = 0;

    public GameCamera gameCamera { get; private set; }
    public Player player;
    public PlayerInput playerInput;
    public Transform enemyContainer;
    public Crosshair crosshair;
    public MusicManager musicManager { get; private set; }

    SceneLoader sceneLoader;

    public static GameSession Instance { get; private set; }

    
    bool killYourself = false;

    private void Awake()
    {
        // Ensure there's only one instance
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
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
        playerInput = FindObjectOfType<PlayerInput>();
        musicManager = FindObjectOfType<MusicManager>();
        gameCamera = FindObjectOfType<GameCamera>();
        sceneLoader = FindObjectOfType<SceneLoader>();
        crosshair = FindObjectOfType<Crosshair>();

        gameCamera.SetPrimaryTarget(player.transform);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.buildIndex);
        if (scene.buildIndex == 0)
        {
            killYourself = true;
            return;
        }

        musicManager = FindObjectOfType<MusicManager>();
        gameCamera = FindObjectOfType<GameCamera>();
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    GameState previousState;
    private void Update()
    {
        if (killYourself)
        {
            Die();
        }
        if (state != previousState)
        {
            OnNewState.Invoke();
        }
        previousState = state;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }


    public void SetState(GameState state)
    {
        GameSession.state = state;
    }

    public void LoadNextLevel()
    {
        levelIndex++;
        sceneLoader.LoadLevel1();
    }

    void Die()
    {
        Debug.Log("Killing GameSession");
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}


