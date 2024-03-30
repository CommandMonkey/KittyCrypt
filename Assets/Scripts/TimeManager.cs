using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class TimeManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuContainer;

    public bool gamePaused;

    private void Awake()
    {
        pauseMenuContainer.SetActive(false);
    }

    private void Start()
    {
        FindObjectOfType<UserInput>().onTogglePause.AddListener(OnTogglePause);
    }

    private void OnTogglePause()
    {
        TogglePause();
    }

    //-------------------------------------------------------------------------------------------------------------------
    // Pausing
    //-------------------------------------------------------------------------------------------------------------------

    public void Pause()
    {
        if (!gamePaused) TogglePause();
    }
    public void UnPause()
    {
        if (gamePaused) TogglePause();
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
