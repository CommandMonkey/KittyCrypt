using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuContainer;

    public bool gamePaused;

    private GameSession gameSession;
    private UserInput userInput;


    private void Awake()
    {
        pauseMenuContainer.SetActive(false);
    }

    private void Start()
    {
        gameSession = GameSession.Instance;
        userInput = gameSession.userInput;

        userInput.onTogglePause.AddListener(OnTogglePause);
    }

    private void OnTogglePause()
    {
        if (GameSession.state != GameSession.GameState.Loading)
        {
            TogglePause();
        }
    }

    //-------------------------------------------------------------------------------------------------------------------
    // Pausing
    //-------------------------------------------------------------------------------------------------------------------

    public void Pause(float delay = 0)
    {
        if (!gamePaused) Invoke("TogglePause", delay);
    }
    public void UnPause(float delay = 0)
    {
        if (gamePaused) Invoke("TogglePause", delay);
    }

    void TogglePause()
    {
        gamePaused = !gamePaused;

        pauseMenuContainer.SetActive(gamePaused);

        UpdateTimeScale();
    }



    /////////////////
    // TimeScale
    //

    public void UpdateTimeScale()
    {
        Time.timeScale = (gamePaused) ? 0f : 1f;
    }
}
