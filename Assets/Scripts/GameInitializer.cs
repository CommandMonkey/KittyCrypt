using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] GameObject GameSessionPrefab;
    [Header("Gamemode Gamedata")]
    [SerializeField] GameSessionData storyEasySettings;
    [SerializeField] GameSessionData storyMediumSettings;
    [SerializeField] GameSessionData storyHardSettings;
    [SerializeField] GameSessionData endlessEasySettings;
    [SerializeField] GameSessionData endlessMediumSettings;
    [SerializeField] GameSessionData endlessHardSettings;
    [Header("Button References")]

    private GameMode gameMode;
    private GameDifficulty difficulty;

    SceneLoader sceneLoader;

    private void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    public void SetGameMode(GameMode gameMode)
    {
        this.gameMode = gameMode;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        
    }

    public void StartGame()
    { 
        if (gameMode == GameMode.tutorial)
        {
            sceneLoader.load
            return;
        }
        GameSessionData sessionSettings = GetSessionDataOfType(gameMode, difficulty);
        Instantiate(GameSessionPrefab);
    }

    private GameSessionData GetSessionDataOfType(GameMode gameMode, GameDifficulty difficulty)
    {
        GameSessionData result = null;
        if (gameMode == GameMode.story) 
        {
            result =    difficulty == GameDifficulty.easy ? storyEasySettings :
                        difficulty == GameDifficulty.medium ? storyMediumSettings :
                        storyHardSettings;
        } 
        else if (gameMode == GameMode.endless)
        {
            result =    difficulty == GameDifficulty.easy ? endlessEasySettings :
                        difficulty == GameDifficulty.medium ? endlessMediumSettings :
                        endlessHardSettings;
        }
        return result;
    }
}
