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

    [Header("Load Settings")]
    public static GameState state = GameState.Loading;
    [NonSerialized] public GameMode gameMode;
    [NonSerialized] public GameSessionData.LevelSettings levelSettings;
    public GameSessionData gameSessionData;
    [NonSerialized] public UnityEvent onEnemyKill;
    [NonSerialized] public UnityEvent OnNewState;
    [NonSerialized] public UnityEvent onSceneloaded;

    // Stats
    [NonSerialized] public int levelIndex = 0;
    [NonSerialized] public float timePlayed = 0f;
    [NonSerialized] public int enemiesKilled = 0;
    [NonSerialized] public int roomsCleared = 0;
    [NonSerialized] public int damageTaken = 0;

    private bool killYourself = false;
    [NonSerialized] public bool playerIsShooting = false;

    public GameCamera GameCamera { get; private set; }
    public Player Player;
    public GameObject angryFace;
    public PlayerInput playerInput;
    public Transform enemyContainer;
    public Crosshair crosshair;
    public DeathScreen deathScreen;
    public ReloadCircleFollowCursor reloadCircle;
    public UserInput userInput;
    public MusicManager musicManager { get; private set; }

    private SceneLoader sceneLoader;
    public static GameSession Instance { get; private set; }

    private void Awake()
    {
        InitializeSingleton();
        InitializeEvents();
        reloadCircle = FindObjectOfType<ReloadCircleFollowCursor>();
    }

    private void Start()
    {
        InitializeReferences();
        ResetUIElements();

        levelSettings = gameSessionData.GetLevelData(levelIndex);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (killYourself)
        {
            Die();
            return;
        }

        if (state != previousState)
        {
            OnNewState.Invoke();
        }
        previousState = state;

        SetCursorOrCrosshair();
        SetPlayerAngry();

        if (state == GameState.Running && !Player.isDead)
        {
            timePlayed += Time.deltaTime;
        }
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
            gameObject.SetActive(false);
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

    private void InitializeReferences()
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

        GameCamera?.SetPrimaryTarget(Player.transform);
    }

    private void ResetUIElements()
    {
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

        // Stop Looping
        if (!gameSessionData.looping)
        {
            if (levelIndex > gameSessionData.levelDatas.Count)
            {
                BackToMenu();
                return;
            }
        }
        levelSettings = gameSessionData.GetLevelData(levelIndex);

        musicManager = FindObjectOfType<MusicManager>();
        GameCamera = FindObjectOfType<GameCamera>();
        sceneLoader = FindObjectOfType<SceneLoader>();

        onSceneloaded.Invoke();
        InitializeEvents();
    }

    public void Die()
    {
        crosshair.gameObject.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        gameObject.SetActive(false);
        Destroy(gameObject);
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

    public void BackToMenu()
    {
        sceneLoader.LoadMainMenu();
    }

    public void RestartGame()
    {
        levelIndex = 0;
        sceneLoader.LoadLevel1();
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

    private IEnumerator PlayerAngryDisable()
    {
        yield return new WaitForSeconds(1f);
        angryFace.gameObject.SetActive(false);
    }

    private GameState previousState;
}
