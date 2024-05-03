using UnityEngine;
using UnityEngine.UI;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] GameObject GameSessionPrefab;
    [Header("Gamemode Gamedata")]
    [SerializeField] GameSessionData storyEasySettings;
    [SerializeField] GameSessionData storyMediumSettings;
    [SerializeField] GameSessionData storyHardSettings;
    [SerializeField] GameSessionData endlessEasySettings;
    [SerializeField] GameSessionData endlessMediumSettings;
    [SerializeField] GameSessionData endlessHardSettings;
    [Header("Button References")]
    [SerializeField] Image tutorialSelectButton;
    [SerializeField] Image storySelectButton;
    [SerializeField] Image endlessSelectButton;

    private GameMode gameMode;
    private GameDifficulty difficulty;

    SceneLoader sceneLoader;

    private void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    public void SetGameMode(GameMode gameMode)
    {
        this.gameMode = gameMode;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        endlessSelectButton.gameObject.SetActive(false);
        storySelectButton.gameObject.SetActive(false);
        endlessSelectButton.gameObject.SetActive(false);

        if (gameMode == GameMode.tutorial)
        {
            endlessSelectButton.gameObject.SetActive(true);
        }
        else if (gameMode == GameMode.story)
        {
            storySelectButton.gameObject.SetActive(true);
        }
        else
        {
            endlessSelectButton.gameObject.SetActive(true);
        }
    }
    

    // Called by Start Button
    public void StartGame()
    {
        if (gameMode == GameMode.tutorial)
        {
            sceneLoader.LoadTutorial();
        }
        else //if (gameMode == GameMode.story || gameMode == GameMode.endless)
        {
            sceneLoader.LoadLevel1();
            CreateGameSession();
        }
    }

    private void CreateGameSession()
    {
        GameSessionData sessionSettings = GetSessionDataOfType(gameMode, difficulty);
        Instantiate(GameSessionPrefab);
    }

    private GameSessionData GetSessionDataOfType(GameMode gameMode, GameDifficulty difficulty)
    {
        GameSessionData result = null;
        if (gameMode == GameMode.story)
        {
            result = difficulty == GameDifficulty.easy ? storyEasySettings :
                        difficulty == GameDifficulty.medium ? storyMediumSettings :
                        storyHardSettings;
        }
        else if (gameMode == GameMode.endless)
        {
            result = difficulty == GameDifficulty.easy ? endlessEasySettings :
                        difficulty == GameDifficulty.medium ? endlessMediumSettings :
                        endlessHardSettings;
        }
        return result;
    }
}
