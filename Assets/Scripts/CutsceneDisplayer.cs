using System;
using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneDisplayer : MonoBehaviour
{
    [SerializeField] float coolDownTime;
    [SerializeField] Animator[] Cutscene;

    GameSession gameSession;
    UserInput userInput;
    SceneLoader sceneLoader;

    int currentImageIndex = 0;
    bool isCoolingDown = false;

    void Start()
    {
        gameSession = GameSession.Instance;
        userInput = gameSession.userInput;
        sceneLoader = FindObjectOfType<SceneLoader>();
        // Subscribe to Input events

        userInput.onDash.AddListener(OnNextImage);
        userInput.onTogglePause.AddListener(EndCutscene);
    }


    void OnNextImage()
    {
        if (isCoolingDown) return; 

        if (currentImageIndex > Cutscene.Length-2)
        {
            EndCutscene();
        }
        else
        {
            UpdateImage();
        }
        currentImageIndex++;
        StartCoroutine(CooldownRoutine());
    }


    void UpdateImage()
    {
        try 
        {
            Cutscene[currentImageIndex].SetTrigger("FadeOut");
            if (currentImageIndex > 0) Cutscene[currentImageIndex-1].gameObject.SetActive(false);
        }
        catch (Exception e) 
        {
            Debug.LogWarning("NO CUTSCENE FOUND SKIPPING TO NEXT, e: " + e);
            OnNextImage();
        }
    }

    void EndCutscene()
    {
        Debug.Log("EndCUTSCENE");
        //Cutscene[currentImageIndex].SetTrigger("FadeOut");
        sceneLoader.LoadLevel1(true);
    }

    IEnumerator CooldownRoutine()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(coolDownTime);
        isCoolingDown = false;
    }
}
