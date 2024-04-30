using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueInteractable : MonoBehaviour, IInteractable
{
    // serialize fields
    [SerializeField] GameObject dialogueTextPrefab;
    [SerializeField] string promptText;
    [SerializeField] string dialogueText = "";
    [SerializeField] float dialogueDuration = 0f;

    [SerializeField] public string interactPrompt {  get; set; }
    public bool canInteract { get; set; } = true;



    private void Start()
    {
        interactPrompt = promptText;
        canInteract = true;
    }


    public void Interact()
    {
        StartCoroutine(DisplayDialogueRutine());
    }

    IEnumerator DisplayDialogueRutine()
    {
        canInteract = false;
        TMP_Text textInstance = Instantiate(dialogueTextPrefab,transform.position, Quaternion.identity, transform.parent).GetComponent<TMP_Text>();
        textInstance.text = dialogueText;

        yield return new WaitForSeconds(dialogueDuration);

        canInteract = true;
        Destroy(textInstance.gameObject);
    }
}