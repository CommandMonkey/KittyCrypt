using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] TMP_Text interactablePromptText;

    List<GameObject> interactablesInRange = new List<GameObject>();
    [SerializeField] bool anyInteractablesInRange = false;

    Transform closestInteractable;
    Transform preciousClosestInteractable;

    // cached references
    Camera mainCamera;


    private void Start()
    {
        interactablePromptText = GameObject.FindGameObjectWithTag("InteractPromptText").GetComponent<TMP_Text>();
        UserInput userInput = FindObjectOfType<UserInput>();
        mainCamera = Camera.main;
        // Setup Input
        userInput.onInteract.AddListener(OnInteract);
    }

    private void Update()
    {
        if (!anyInteractablesInRange)
        {
            interactablePromptText.text = "";
            return;
        }

        closestInteractable = GetClosestInteractable()?.transform;

        if (closestInteractable != null )
        {
            UpdateInteractPromptText();
            UpdateInteractPromptPos();
        }
        else
        {
            interactablePromptText.text = "";
        }
        

    }

    private void UpdateInteractPromptPos()
    {
        interactablePromptText.transform.position = closestInteractable.transform.position;
    }

    private void UpdateInteractPromptText()
    {
        interactablePromptText.text = closestInteractable.GetComponent<IInteractable>().interactPrompt;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && !interactablesInRange.Contains(other.gameObject))
        {
            anyInteractablesInRange = true;
            // set any interactables in range
            interactablesInRange.Add(other.gameObject);
            Debug.Log("interactable count: " + interactablesInRange.Count);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactablesInRange.Contains(other.gameObject))
        {
            interactablesInRange.Remove(other.gameObject);
            // set any interactables in range
            Debug.Log("interactable count: " + interactablesInRange.Count);
            if (interactablesInRange.Count <= 0)
            {
                anyInteractablesInRange = false;
            }
        }
    }

    // ////// Interact ////// //
    void OnInteract()
    {
        if (!anyInteractablesInRange) return;
        closestInteractable.GetComponent<IInteractable>().Interact();
    } 




    // /////// Helper /////// //

    public void UnRegisterInteractable(GameObject interactable)
    {
        if(interactablesInRange.Contains(interactable))
        {
            interactablesInRange.Remove(interactable);
            if (interactablesInRange.Count <= 0)
            {
                anyInteractablesInRange = false;
            }
        }
    }

    private GameObject GetClosestInteractable()
    {
        GameObject closestInteractable = null;
        float smallestMagnitude = float.MaxValue;

        foreach (GameObject interactable in interactablesInRange)
        {
            if (interactable.GetComponent<IInteractable>().canInteract)
            {
                float magnitude = (interactable.transform.position - transform.position).sqrMagnitude; // Using sqrMagnitude for performance

                if (magnitude < smallestMagnitude)
                {
                    closestInteractable = interactable;
                    smallestMagnitude = magnitude;
                }
            }
        }

        return closestInteractable;
    }
}