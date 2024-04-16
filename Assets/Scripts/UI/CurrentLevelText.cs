using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentLevelText : MonoBehaviour
{
    TextMeshProUGUI levelText;

    private void Awake()
    {
        levelText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        levelText.text = "LEVEL " + (GameSession.Instance.levelIndex + 1).ToString();
        Debug.Log(GameSession.Instance.levelIndex.ToString());
    }
}
