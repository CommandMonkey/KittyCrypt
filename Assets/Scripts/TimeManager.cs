using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuContainer;

    public bool gamePaused;

    private GameSession gameSession;

    private void Awake()
    {
        pauseMenuContainer.SetActive(false);
    }

    private void Start()
    {
        FindObjectOfType<UserInput>().onTogglePause.AddListener(OnTogglePause);
        gameSession = FindObjectOfType<GameSession>();
    }

    private void OnTogglePause()
    {
        if (gameSession.state != GameSession.GameState.Loading) TogglePause();
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
