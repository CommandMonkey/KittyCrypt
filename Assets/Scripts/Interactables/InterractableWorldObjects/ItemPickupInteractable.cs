

using UnityEngine;

public class ItemPickupInteractable : MonoBehaviour, IInteractable
{
    public GameObject player { get; set; }
    public string interactPrompt { get; set; }
    public bool caninteract { get; set; }

    public void Interact()
    {
        
    }
}