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
    Player player;

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

    public void Retry()
    {
        LoadLevel1();
    }

    IEnumerator FadeAndLoadSceneRoutine(int sceneIndex)
    {
        if (gameSession != null) gameSession.SetState(GameSession.GameState.Loading);
        transitionAnimator.SetBool("isLoading", true);

        yield return new WaitForSeconds(2f);
        if (gameSession.player.isDead)
        {
            Destroy(gameSession.gameObject);
        }

        if(gameSession != null) gameSession.SetState(GameSession.GameState.Running);
        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator LoadLevel()
    {
        transitionAnimator.SetBool("isLoading", true);
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        transitionAnimator.SetBool("isLoading", false);
        if(gameSession != null) gameSession.SetState(GameSession.GameState.Running);

        generationScreen.gameObject.SetActive(false);
    }
}
