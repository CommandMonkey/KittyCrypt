using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExitInteractable : MonoBehaviour, IInteractable
{

    [SerializeField] Sprite OpenSprite;
    [SerializeField] Sprite ClosedSprite;

    public string interactPrompt { get; set; }
    public bool canInteract { get; set; }
    public bool interactable;

    GameSession gameSession;
    SpriteRenderer spriteRenderer;

    bool hasWarned = false;

    private void Start()
    {
        interactPrompt = "Go to the next level(E)";
        gameSession = GameSession.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log(spriteRenderer != null);
        SetInteractable();
        UpdateSprite();
    }

    public void Interact()
    {
        if (hasWarned)
        {
            gameSession.LoadNextLevel();
            interactPrompt = "";
        }
        else
        {
            interactPrompt = "Are you sure?(E)";
            hasWarned = true;
        }
    }

    public void SetInteractable()
    {
        canInteract = true;
        interactable = true;
        UpdateSprite();
    }

    public void SetUnInteractable()
    {
        canInteract = false;
        interactable = false;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = canInteract ? OpenSprite : ClosedSprite;
    }
}
