using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] int mainMenuBuildIndex;
    [SerializeField] int tutorialBuildIndex;
    [SerializeField] int CutsceneBuildIndex;
    [SerializeField] int levelBuildIndex;



    // Cached Component References
    Animator transitionAnimator;
    GameObject generationScreen;
    GameSession gameSession;

    private void Start()
    {
        gameSession = GameSession.Instance;
        transitionAnimator = GetComponentInChildren<Animator>();
        generationScreen = GameObject.Find("GeneratingScreen");
        if (transitionAnimator != null) transitionAnimator.SetBool("isLoading", false);
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

    public void LoadLevel1(bool fade = true)
    {
        if (!fade)
            SceneManager.LoadScene(levelBuildIndex);
        else
            StartCoroutine(FadeAndLoadSceneRoutine(levelBuildIndex));
    }

    public void LoadMainMenu(bool fade = true)
    {
        if (!fade)
            SceneManager.LoadScene(mainMenuBuildIndex);
        else
            StartCoroutine(FadeAndLoadSceneRoutine(mainMenuBuildIndex));
    }

    public void LoadTutorial(bool fade = true)
    {
        if (!fade)
            SceneManager.LoadScene(tutorialBuildIndex);
        else
            StartCoroutine(FadeAndLoadSceneRoutine(tutorialBuildIndex));
    }
    public void LoadCutscene(bool fade = true)
    {
        if (!fade)
            SceneManager.LoadScene(CutsceneBuildIndex);
        else
            StartCoroutine(FadeAndLoadSceneRoutine(CutsceneBuildIndex));
    }

    IEnumerator FadeAndLoadSceneRoutine(int sceneIndex)
    {
        if (gameSession != null) gameSession.SetState(GameSession.GameState.Loading);
        transitionAnimator.SetBool("isLoading", true);

        yield return new WaitForSeconds(2f);    

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

    public void KillGameSession()
    {
        Debug.Log(gameSession.gameObject);
        gameSession.Die();
    }
}
