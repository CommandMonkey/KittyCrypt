using UnityEngine;

public interface IInteractable
{
    string interactPrompt { get; set; } // the text displayed when´closest interactabele in the players interact range
    bool canInteract { get; set; } // if should be able to be interacted with

    void Interact(); // called once the player interacts with this
}
