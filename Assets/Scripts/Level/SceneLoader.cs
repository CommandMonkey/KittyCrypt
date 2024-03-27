using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    TimeManager timeManager;

    private void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadSettings()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadMainMenu()
    {
        timeManager.UnPause();
        SceneManager.LoadScene(0);
    }

}
