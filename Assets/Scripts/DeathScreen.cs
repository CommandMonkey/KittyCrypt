using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    GameSession gameSession;
    [SerializeField] TextMeshProUGUI timeSurvived;
    [SerializeField] TextMeshProUGUI finalFloor;
    [SerializeField] TextMeshProUGUI roomsCleared;
    [SerializeField] TextMeshProUGUI enemiesKilled;
    [SerializeField] TextMeshProUGUI damageTaken;


    private void OnEnable()
    {
        gameSession = GameSession.Instance;
        if (gameSession != null)
        {   
            var ts = TimeSpan.FromSeconds(gameSession.timePlayed);
            timeSurvived.text = "TIME SURVIVED: " + string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            finalFloor.text = "FINAL FLOOR: " + (gameSession.levelIndex + 1).ToString();
            roomsCleared.text = "ROOMS CLEARED: " + gameSession.roomsCleared.ToString();
            enemiesKilled.text = "ENEMIES KILLED: " + gameSession.enemiesKilled.ToString();
            damageTaken.text = "DAMAGE TAKEN: " + gameSession.damageTaken.ToString();
        }
    }
}
