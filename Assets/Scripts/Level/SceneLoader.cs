using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Cached Component References
    Animator transitionAnimator;
    GameObject generationScreen;
    GameSession gameSession;

    private void Start()
    {
        gameSession = GameSession.Instance;
        transitionAnimator = GetComponentInChildren<Animator>();
        generationScreen = GameObject.Find("GeneratingScreen");
        transitionAnimator.SetBool("isLoading", false);
        if (generationScreen != null)
        {
            gameSession.SetState(GameSession.GameState.Loading);
            StartCoroutine(LoadLevel());
        }
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(FadeAndLoadSceneRoutine(currentSceneIndex + 1));
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }

    public void LoadLevel1()
    {
        StartCoroutine(FadeAndLoadSceneRoutine(2));
    }

    public void LoadSettings()
    {
        StartCoroutine(FadeAndLoadSceneRoutine(1));
    }

    public void LoadMainMenu()
    {
        StartCoroutine(FadeAndLoadSceneRoutine(0));
    }

    IEnumerator FadeAndLoadSceneRoutine(int sceneIndex)
    {
        transitionAnimator.SetBool("isLoading", true);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator LoadLevel()
    {
        transitionAnimator.SetBool("isLoading", true);
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        transitionAnimator.SetBool("isLoading", false);
        gameSession.SetState(GameSession.GameState.Running);

        generationScreen.gameObject.SetActive(false);
    }
}
