using UnityEngine;

public interface IInteractable
{
    string interactPrompt { get; set; }
    bool canInteract { get; set; }

    void Interact();
}
