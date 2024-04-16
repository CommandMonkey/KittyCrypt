using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    public GameSessionData.LevelSettings levelSettings;

    public GameSessionData gameSessionData;
    [NonSerialized] public UnityEvent onEnemyKill;
    [NonSerialized] public UnityEvent OnNewState;
    [NonSerialized] public UnityEvent onSceneloaded;


    public int levelIndex = 0;
    
    bool killYourself = false;
    public bool playerIsShooting = false;

    public GameCamera gameCamera { get; private set; }
    public Player player;
    public GameObject angryFace; 
    public PlayerInput playerInput;
    public Transform enemyContainer;
    public Crosshair crosshair;
    public ReloadCircleFollowCursor reloadCircle;
    public UserInput userInput { get; private set; }
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
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        angryFace = GameObject.Find("Angry");
        playerInput = FindObjectOfType<PlayerInput>();
        musicManager = FindObjectOfType<MusicManager>();
        gameCamera = FindObjectOfType<GameCamera>();
        sceneLoader = FindObjectOfType<SceneLoader>();
        crosshair = FindObjectOfType<Crosshair>();
        userInput = GetComponentInChildren<UserInput>();
        

        gameCamera.SetPrimaryTarget(player.transform);
        crosshair.gameObject.SetActive(false);
        angryFace.gameObject.SetActive(false);

        levelSettings = gameSessionData.GetLevelData(levelIndex);

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
        gameCamera = FindObjectOfType<GameCamera>();
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
    }

    private void Die()
    {
        crosshair.gameObject.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("Killing GameSession");
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
        yield return new WaitForSeconds(5f);
        angryFace.gameObject.SetActive(false);
    }
}


