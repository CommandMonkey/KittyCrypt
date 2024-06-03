using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class BossDoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] Sprite frontClosedDoorSprite;
    [SerializeField] Sprite frontOpenDoorSprite;
    [SerializeField] Sprite sideClosedDoorSprite;
    [SerializeField] Sprite sideOpenDoorSprite;
    [SerializeField] GameObject dialogueTextPrefab;
    [SerializeField] float dialogueDuration = 2f;

    public string interactPrompt { get; set; }
    public bool canInteract { get; set; }

    public Direction direction;

    bool doorOpen = true;


    BoxCollider2D collisionCollider;
    BoxCollider2D triggerCollider;
    SpriteRenderer spriteRenderer;
    RoomManager roomManager;
    UICanvas uiCanvas;
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        collisionCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        triggerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        roomManager = FindObjectOfType<RoomManager>();
        uiCanvas = FindObjectOfType<UICanvas>();
        player = GameSession.Instance.Player;

        canInteract = true;

        interactPrompt = "Rat Kings lair,\nTry To Open";   
        CloseDoor();
        SetDoorCollisionSize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        if (player.hasKey)
        {
            OpenDoor();
            player.hasKey = false;
            FollowKey followKey = FindObjectOfType<FollowKey>();
            if (followKey != null) 
            {
                Destroy(followKey.gameObject);
            }
            canInteract = false;
            
            uiCanvas.directionPointer.gameObject.SetActive(false);
            uiCanvas.directionPointer.target = null;
        }
        else
        {
            StartCoroutine(DisplayDialogueRutine("Door Is Locked,\n Find the Key"));
            KeyPickupInteractable keyPickup = FindObjectOfType<KeyPickupInteractable>();
            if (keyPickup != null)
            {
            uiCanvas.directionPointer.gameObject.SetActive(true);
            uiCanvas.directionPointer.target = keyPickup.transform;
            }
        }

    }

    private void SetDoorCollisionSize()
    {
        int numDir = (int)direction;
        collisionCollider.size =
            numDir == 0 || numDir == 2 ? new Vector2(3, 3) : new Vector2(1, 3);
        collisionCollider.offset =
            numDir == 0 || numDir == 2 ? new Vector2(0, -1) : Vector2.zero;
    }

    public void CloseDoor()
    {
        if (doorOpen) ToggleDoor();
    }

    public void OpenDoor()
    {
        if (!doorOpen) ToggleDoor();
    }

    protected void ToggleDoor()
    {
        doorOpen = !doorOpen;

        collisionCollider.gameObject.SetActive(!doorOpen);
        //animator.SetBool("doorOpen", doorOpen);
        UpdateSprite();
    }

    void UpdateSprite()
    {
        if ((int)direction == 0 || (int)direction == 2)
        {
            spriteRenderer.sprite = doorOpen ? frontOpenDoorSprite : frontClosedDoorSprite;
        }
        else
        {
            spriteRenderer.sprite = doorOpen ? sideOpenDoorSprite : sideClosedDoorSprite;
        }
    }

    IEnumerator DisplayDialogueRutine(string text)
    {
        canInteract = false;
        TMP_Text textInstance = Instantiate(dialogueTextPrefab, transform.position, Quaternion.identity, transform.parent).GetComponent<TMP_Text>();
        textInstance.text = text;

        yield return new WaitForSeconds(dialogueDuration);

        canInteract = true;
        Destroy(textInstance.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        roomManager.onEntranceExit.Invoke();
    }
}
