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

    private void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateSprite();
    }

    public void Interact()
    {
        gameSession.NextLevelLoad();
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