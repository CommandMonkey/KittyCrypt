using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuContainer;

    public bool gamePaused;

    private UserInput userInput;
    private GameSession gameSession;

    private void Awake()
    {
        pauseMenuContainer.SetActive(false);
    }

    private void Start()
    {
        userInput = FindObjectOfType<UserInput>();
        gameSession = GameSession.Instance;

        userInput.onTogglePause.AddListener(OnTogglePause);
    }

    private void OnTogglePause()
    {
        if (GameSession.state != GameSession.GameState.Loading) TogglePause();
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
