using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] GameObject GameSessionPrefab;
    [Header("GamemodeGamedata")]
    [SerializeField] GameSessionData StoryEasySettings;
    [SerializeField] GameSessionData StoryMediumSettings;
    [SerializeField] GameSessionData StoryHardSettings;
    [SerializeField] GameSessionData EndlessEasySettings;
    [SerializeField] GameSessionData EndlessMediumSettings;
    [SerializeField] GameSessionData EndlessHardSettings;


    private GameMode gameMode;



    public void SetGameMode(GameMode gameMode)
    {

    }

    public void InstanciateGameSession()
    {
        Instantiate(GameSessionPrefab);
    }
}
