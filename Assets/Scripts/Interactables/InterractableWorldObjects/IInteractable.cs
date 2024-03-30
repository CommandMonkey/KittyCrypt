using UnityEngine;

public interface IInteractable
{
    GameObject player { get; set; }

    string interactPrompt { get; set; }

    bool caninteract { get; set; }

    void Interact();
}
