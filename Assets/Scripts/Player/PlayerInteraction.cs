using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] TMP_Text interactablePromptText;

    List<GameObject> interactablesInRange = new List<GameObject>();
    [SerializeField] bool anyInteractablesInRange = false;

    Transform closestInteractable;

    // cached references
    UserInput userInput;
    CircleCollider2D circleCollider;
    GameSession gameSession;


    private void Start()
    {
        interactablePromptText = GameObject.FindGameObjectWithTag("InteractPromptText")?.GetComponent<TMP_Text>();
        userInput = FindObjectOfType<UserInput>();
        circleCollider = GetComponent<CircleCollider2D>();
        gameSession = FindObjectOfType<GameSession>();
        // Setup Input
        userInput.onInteract.AddListener(OnInteract);

        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        userInput.onInteract.AddListener(OnInteract);
    }

    private void Update()
    {
        if (interactablePromptText == null) return;
        if (!anyInteractablesInRange || GameSession.state != GameSession.GameState.Running)
        {
            interactablePromptText.text = "";
            return;
        }

        //foreach (GameObject t in interactablesInRange)
        //{
        //    if (t != null)
        //    {
        //        if (!circleCollider.OverlapPoint(t.transform.position))
        //        {
        //            UnRegisterInteractable(t);
        //        }
        //    }

        //}

        GameObject interactable = GetClosestInteractable();
        if (interactable != null)
        {
            closestInteractable = interactable.transform;
        }
        else
        {
            closestInteractable = null;
        }


        if (closestInteractable != null)
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
        interactablePromptText.text = closestInteractable.GetComponent<IInteractable>().interactPrompt + "(" + GetInteractKey() + ")";
    }

    private string GetInteractKey()
    {
        string currentControllScheme = gameSession.playerInput.currentControlScheme;
        if (currentControllScheme == "Keyboard and mouse")
        {
            return "E";
        }
        else if (currentControllScheme == "PlayStation Controller")
        {
            return "X";
        }
        else  if (currentControllScheme == "Xbox Controller")
        {
            return "A";
        }
        else if (currentControllScheme == "Switch Controller")
        {
            return "B";
        }
        return "I DONT KNOW";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && !interactablesInRange.Contains(other.gameObject))
        {
            anyInteractablesInRange = true;
            // set any interactables in range
            interactablesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactablesInRange.Contains(other.gameObject))
        {
            interactablesInRange.Remove(other.gameObject);
            // set any interactables in range
            if (interactablesInRange.Count <= 0)
            {
                anyInteractablesInRange = false;
            }
        }
    }

    // ////// Interact ////// //
    void OnInteract()
    {
        if (closestInteractable == null || GameSession.state != GameSession.GameState.Running) return;
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
                float magnitude = (interactable.transform.position - transform.position).magnitude; // Using sqrMagnitude for performance
                // Debug.Log(magnitude + " , " + circleCollider.radius);
                if (magnitude < circleCollider.radius && magnitude < smallestMagnitude)
                {
                    closestInteractable = interactable;
                    smallestMagnitude = magnitude;
                }
            }
        }

        return closestInteractable;
    }
}