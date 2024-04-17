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
        interactPrompt = "Go to the next level ";
        gameSession = GameSession.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetInteractable();
        UpdateSprite();
        
        InvokeRepeating("CheckIfBossAlive", 1f, 3f);
    }

    void CheckIfBossAlive()
    {
        RatBossBehaviour ratBoss = FindObjectOfType<RatBossBehaviour>();
        if (ratBoss != null)
        {
            if (ratBoss.state == RatBossBehaviour.RatState.dead)
                SetInteractable();
            else
                SetUnInteractable();
        }
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
            interactPrompt = "Are you sure? ";
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
