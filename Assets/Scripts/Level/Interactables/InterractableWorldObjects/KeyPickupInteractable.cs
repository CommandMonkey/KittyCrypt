
using UnityEngine;

public class KeyPickupInteractable : MonoBehaviour, IInteractable
{
    public GameObject replacementPrefab;

    public string interactPrompt { get; set; }
    public bool canInteract { get; set; }

    private void Start()
    {
        interactPrompt = "Rat Key";
        canInteract = true;
    }

    public void Interact()
    {
        gameObject.SetActive(false);
        Instantiate(replacementPrefab, transform.position, transform.rotation);
    }

}

