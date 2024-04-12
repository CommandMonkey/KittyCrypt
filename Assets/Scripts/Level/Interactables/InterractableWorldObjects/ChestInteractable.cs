using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ChestInteractable : MonoBehaviour, IInteractable
{
    public string interactPrompt { get; set ; }
    public bool canInteract { get; set; } = true;

    [SerializeField] Sprite openChest;
    [SerializeField] GameObject pickUpPrefab;
    SpriteRenderer mySpriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        interactPrompt = "E to open chest";
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Interact()
    {
        mySpriteRenderer.sprite = openChest;
        Instantiate(pickUpPrefab, new Vector2(transform.position.x, transform.position.y + .5f), Quaternion.identity);
        canInteract = false;
    }
}
