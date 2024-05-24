using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameMode
{
    notAssigned,
    tutorial,
    story,
    endless
}

public enum GameDifficulty
{
    notAssigned,
    easy,
    medium,
    hard
}

public class GameSession : MonoBehaviour
{
    public enum GameState
    {
        Loading,
        Running,
        Paused
    }

    public static GameSession Instance { get; private set; }

    [Header("Load Settings")]
    public static GameState state = GameState.Loading;
    public GameMode gameMode;
    public GameSessionData.LevelSettings levelSettings;
    public GameSessionData gameSessionData;

    [NonSerialized] public UnityEvent onEnemyKill;
    [NonSerialized] public UnityEvent OnNewState;
    [NonSerialized] public UnityEvent onSceneloaded;

    public int levelIndex = 0;
    public float timePlayed = 0f;
    public int enemiesKilled = 0;
    public int roomsCleared = 0;
    public int damageTaken = 0;

    private bool killYourself = false;
    public bool playerIsShooting = false;

    public GameCamera GameCamera { get; private set; }
    public Player Player;
    public GameObject angryFace;
    public PlayerInput playerInput;
    public Transform enemyContainer;
    public Crosshair crosshair;
    public DeathScreen deathScreen;
    public ReloadCircleFollowCursor reloadCircle;
    public UserInput userInput;
    public RoomManager roomManager;
    public MusicManager musicManager { get; private set; }

    private SceneLoader sceneLoader;
    private GameState previousState;

    private void Awake()
    {
        InitializeSingleton();
        InitializeEvents();
        FindInitialComponents();
        LoadCurrentLevelData();
    }

    private void Start()
    {
        AssignComponentReferences();
        InitializeComponents();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        HandleKillYourself();
        UpdateGameState();
        SetCursorOrCrosshair();
        SetPlayerAngry();
        UpdateTimePlayed();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void InitializeEvents()
    {
        onEnemyKill = new UnityEvent();
        OnNewState = new UnityEvent();
        onSceneloaded = new UnityEvent();
    }

    private void FindInitialComponents()
    {
        reloadCircle = FindObjectOfType<ReloadCircleFollowCursor>();
    }

    private void LoadCurrentLevelData()
    {
        levelSettings = gameSessionData.GetLevelData(levelIndex);
    }

    private void AssignComponentReferences()
    {
        Player = FindObjectOfType<Player>();
        angryFace = GameObject.Find("Angry");
        musicManager = FindObjectOfType<MusicManager>();
        GameCamera = FindObjectOfType<GameCamera>();
        sceneLoader = FindObjectOfType<SceneLoader>();
        crosshair = FindObjectOfType<Crosshair>();
        deathScreen = FindObjectOfType<DeathScreen>();
        userInput = GetComponentInChildren<UserInput>();
        playerInput = userInput.GetComponent<PlayerInput>();
    }

    private void InitializeComponents()
    {
        GameCamera?.SetPrimaryTarget(Player.transform);
        crosshair.gameObject.SetActive(false);
        deathScreen.gameObject.SetActive(false);
        angryFace.gameObject.SetActive(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            killYourself = true;
            return;
        }

        InitializeEvents();
        LoadCurrentLevelData();
        AssignComponentReferences();
        onSceneloaded.Invoke();
    }

    private void HandleKillYourself()
    {
        if (killYourself)
        {
            Die();
        }
    }

    private void UpdateGameState()
    {
        if (state != previousState)
        {
            OnNewState.Invoke();
        }
        previousState = state;
    }

    private void SetCursorOrCrosshair()
    {
        if (playerInput.currentControlScheme != "Keyboard and mouse")
        {
            crosshair.gameObject.SetActive(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            crosshair.gameObject.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void SetPlayerAngry()
    {
        if (!playerIsShooting) return;

        StopAllCoroutines();
        angryFace.gameObject.SetActive(true);
        StartCoroutine(PlayerAngryDisable());
    }

    private void UpdateTimePlayed()
    {
        if (state == GameState.Running && !Player.isDead)
        {
            timePlayed += Time.deltaTime;
        }
    }

    public void Die()
    {
        crosshair.gameObject.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void SetState(GameState newState)
    {
        state = newState;
    }

    public void LoadNextLevel()
    {
        levelIndex++;
        sceneLoader.LoadLevel1();
    }

    private IEnumerator PlayerAngryDisable()
    {
        yield return new WaitForSeconds(1f);
        angryFace.gameObject.SetActive(false);
    }
}
