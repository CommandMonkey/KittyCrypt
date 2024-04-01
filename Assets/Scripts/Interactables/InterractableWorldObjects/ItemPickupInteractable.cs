

using System;
using UnityEngine;

public class ItemPickupInteractable : MonoBehaviour, IInteractable
{
    public GameObject item;
    public string interactPrompt { get; set; }
    public bool canInteract { get; set; } = true;

    private void Start()
    {
        interactPrompt = "E to pickup " + item.name;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null )
        {
            sr.sprite = FindSpriteRendererInChildren(transform)?.sprite;
        }
    }

    private SpriteRenderer FindSpriteRendererInChildren(Transform parent)
    {
        SpriteRenderer spriteRenderer = parent.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            return spriteRenderer;
        }

        foreach (Transform child in parent)
        {
            SpriteRenderer childSpriteRenderer = FindSpriteRendererInChildren(child);
            if (childSpriteRenderer != null)
            {
                return childSpriteRenderer;
            }
        }

        return null;
    }

    public void Interact()
    {
        FindObjectOfType<PlayerInventory>().AddWeapon(item);
        FindObjectOfType<PlayerInteraction>().UnRegisterInteractable(gameObject);
        Destroy(gameObject);
    }
}