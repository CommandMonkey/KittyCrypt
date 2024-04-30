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

    // Start is called before the first frame update
    void Start()
    {
        gameSession = GameSession.Instance;
    }

    private void OnEnable()
    {
        if (gameSession != null)
        {
            var ts = TimeSpan.FromSeconds(gameSession.timePlayed);
            timeSurvived.text = "TIME SURVIVED: " + string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            finalFloor.text = "FINAL FLOOR: " + (gameSession.levelIndex + 1).ToString();
        }
    }
}
