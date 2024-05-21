using UnityEngine;
using UnityEngine.UI;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] GameObject GameSessionPrefab;
    [Header("Gamemode Gamedata")]
    [SerializeField] GameSessionData storySettings;
    [SerializeField] GameSessionData endlessEasySettings;
    [SerializeField] GameSessionData endlessMediumSettings;
    [SerializeField] GameSessionData endlessHardSettings;
    [Header("Button Refs")]
    [SerializeField] Button startButton;
    [SerializeField] Button quitButton;
    [Header("Start Button")]
    [SerializeField] Image startButtonImage;
    [SerializeField] Color startButtonClickableColor;

    public GameMode gameMode;
    private GameDifficulty endlessdifficulty = GameDifficulty.medium;

    SceneLoader sceneLoader;

    private void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            startButton.enabled = true;
            quitButton.enabled = true;
            gameObject.SetActive(false);
        }
    }


    public void SetGameModeTutorial()
    {
        SetGameMode(GameMode.tutorial);
    }

    public void SetGameModeStory()
    {
        SetGameMode(GameMode.story);
    }

    public void SetGameModeEndless()
    {
        SetGameMode(GameMode.endless);
    }

    void SetGameMode(GameMode gameMode)
    {
        this.gameMode = gameMode;
        UpdateStartButton();
    }

    public void SetEndlessDifficultyEasy()
    {
        this.endlessdifficulty = GameDifficulty.easy;
    }
    public void SetEndlessDifficultyMedium()
    {
        this.endlessdifficulty = GameDifficulty.medium;
    }
    public void SetEndlessDifficultyHard()
    {
        this.endlessdifficulty = GameDifficulty.hard;
    }


    void UpdateStartButton()
    {
        if (gameMode != GameMode.notAssigned)
        {
           startButtonImage.color = startButtonClickableColor;
        }
    }

    public void OnStartButtonPressed()
    {
        if (startButtonImage.color == startButtonClickableColor)
        {
            StartGame();
        }
    }

    

    // Called by Start Button
    public void StartGame()
    {
        if (gameMode == GameMode.tutorial)
        {
            sceneLoader.LoadTutorial(true);
        }
        else if (gameMode == GameMode.story)
        {
            sceneLoader.LoadCutscene(true);
            CreateGameSession();
        }
        else if (gameMode == GameMode.endless)
        {
            sceneLoader.LoadLevel1(true);
            CreateGameSession();
        }
    }

    private void CreateGameSession()
    {
        GameSessionData sessionSettings = GetSessionDataOfType();
        Instantiate(GameSessionPrefab).GetComponent<GameSession>().gameSessionData = sessionSettings;
    }

    private GameSessionData GetSessionDataOfType()
    {
        GameSessionData result = null;
        if (gameMode == GameMode.story)
        {
            result = storySettings;
        }
        else if (gameMode == GameMode.endless)
        {
            result = endlessdifficulty == GameDifficulty.easy ? endlessEasySettings :
                     endlessdifficulty == GameDifficulty.medium ? endlessMediumSettings :
                     endlessHardSettings;
        }
        return result;
    }
}
