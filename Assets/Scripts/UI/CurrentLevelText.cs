using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentLevelText : MonoBehaviour
{
    TextMeshProUGUI levelText;
    GameSession gameSession;    

    private void Awake()
    {
        gameSession = GameSession.Instance;
        levelText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (gameSession.gameMode == GameMode.endless)
        {
            levelText.text = "LEVEL: " + (gameSession.levelIndex+1).ToString() + " - " + gameSession.levelSettings.LevelName;
        }
        else
        {
            levelText.text = gameSession.levelSettings.LevelName;
        }

    }
}
