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
    public int levelIndex { get; private set; } = 0;

    public GameCamera gameCamera { get; private set; }
    public Player player;
    public Transform enemyContainer;
    public MusicManager musicManager { get; private set; }

    SceneLoader sceneLoader;

    public static GameSession Instance { get; private set; }

    
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
        musicManager = FindObjectOfType<MusicManager>();
        gameCamera = FindObjectOfType<GameCamera>();
        sceneLoader = FindObjectOfType<SceneLoader>();

        gameCamera.SetPrimaryTarget(player.transform);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            Debug.Log("Killing GameSession");
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        musicManager = FindObjectOfType<MusicManager>();
        gameCamera = FindObjectOfType<GameCamera>();
        sceneLoader = FindObjectOfType<SceneLoader>();
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

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }


    public void SetState(GameState state)
    {
        this.state = state;
    }

    public void LoadNextLevel()
    {
        levelIndex++;
        sceneLoader.LoadLevel1();
    }


}


