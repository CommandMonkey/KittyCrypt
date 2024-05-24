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
    public GameMode gameMode;

    public GameSessionData.LevelSettings levelSettings;

    public GameSessionData gameSessionData;
    [NonSerialized] public UnityEvent onEnemyKill;
    [NonSerialized] public UnityEvent OnNewState;
    [NonSerialized] public UnityEvent onSceneloaded;

    //Stats
    public int levelIndex = 0;
    public float timePlayed = 0f;
    public int enemiesKilled = 0;
    public int roomsCleared = 0;
    public int damageTaken = 0;

    bool killYourself = false;
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
    public
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
        onSceneloaded = new UnityEvent();
        reloadCircle = FindObjectOfType<ReloadCircleFollowCursor>();

        // Get current level data (Based on levelIndex)
        levelSettings = gameSessionData.GetLevelData(levelIndex);
    }

    private void Start()
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

        // Reset default value
        GameCamera?.SetPrimaryTarget(Player.transform);
        crosshair.gameObject.SetActive(false);
        deathScreen.gameObject.SetActive(false);
        angryFace.gameObject.SetActive(false);



        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            killYourself = true;
            return;
        }

        onEnemyKill = new UnityEvent();
        OnNewState = new UnityEvent();
        levelSettings = gameSessionData.GetLevelData(levelIndex);

        musicManager = FindObjectOfType<MusicManager>();
        GameCamera = FindObjectOfType<GameCamera>();
        sceneLoader = FindObjectOfType<SceneLoader>();

        onSceneloaded.Invoke(); 
    }

    GameState previousState;
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
        if(state == GameState.Running && !Player.isDead)
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

    void SetCursorOrCrosshair()
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

    void SetPlayerAngry()
    {
        if (!playerIsShooting) { return; }
        StopAllCoroutines();
        angryFace.gameObject.SetActive(true);
        StartCoroutine(PlayerAngryDisable());
    }

    IEnumerator PlayerAngryDisable()
    {
        yield return new WaitForSeconds(1f);
        angryFace.gameObject.SetActive(false);
    }
}


