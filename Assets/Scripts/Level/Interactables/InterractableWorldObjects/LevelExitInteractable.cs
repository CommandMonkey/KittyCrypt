using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExitInteractable : MonoBehaviour, IInteractable
{

    [SerializeField] Sprite OpenSprite;
    [SerializeField] Sprite ClosedSprite;

    public string interactPrompt { get; set; }
    public bool canInteract { get; set; }

    GameSession gameSession;
    SpriteRenderer spriteRenderer;

    bool hasWarned = false;

    private void Start()
    {
        interactPrompt = "Go to the next level(E)";
        gameSession = GameSession.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log(spriteRenderer != null);

        UpdateSprite();
    }

    public void Interact()
    {
        if (hasWarned)
        {
            gameSession.LoadNextLevel();
        }
        else
        {
            interactPrompt = "Are you sure?(E)";
        }
    }

    public void SetInteractable()
    {
        canInteract = true;
        UpdateSprite();
    }

    public void SetUnInteractable()
    {
        canInteract = false;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = canInteract ? OpenSprite : ClosedSprite;
    }
}
