using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueInteractable : MonoBehaviour, IInteractable
{
    // serialize fields
    [SerializeField] GameObject dialogueTextPrefab;
    [SerializeField] string dialogueText = "";
    [SerializeField] float dialogueDuration = 0f;

    [SerializeField] public string interactPrompt {  get; set; }
    public bool canInteract { get; set; } = true;

    // static to avoid multiple dialogues at once
    static bool dialogueActive = false;

    private void Start()
    {
        interactPrompt = "press E"; 
    }

    private void Update()
    {
        canInteract = !dialogueActive;
    }

    public void Interact()
    {
        StartCoroutine(DisplayDialogueRutine());
    }

    IEnumerator DisplayDialogueRutine()
    {
        dialogueActive = true;
        TMP_Text textInstance = Instantiate(dialogueTextPrefab, transform).GetComponent<TMP_Text>();
        textInstance.text = dialogueText;

        yield return new WaitForSeconds(dialogueDuration);

        dialogueActive = false;
        Destroy(textInstance);
    }
}