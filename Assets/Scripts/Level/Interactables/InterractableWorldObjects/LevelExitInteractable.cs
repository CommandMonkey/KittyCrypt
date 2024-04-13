using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneExit : MonoBehaviour, IInteractable
{
    public string interactPrompt { get; set; }
    public bool canInteract { get; set; }

    GameSession gameSession;

    private void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
    }

    public void Interact()
    {
        gameSession.NextLevelLoad();
    }
}
