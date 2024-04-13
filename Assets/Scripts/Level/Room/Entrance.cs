using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Entrance : MonoBehaviour
{
    public Direction direction;
    [SerializeField] Sprite frontClosedDoorSprite;
    [SerializeField] Sprite frontOpenDoorSprite;
    [SerializeField] Sprite sideClosedDoorSprite;
    [SerializeField] Sprite sideOpenDoorSprite;

    [NonSerialized] public GameObject roomToSpawn;
    [NonSerialized] public List<string> roomFailNames = new List<string>();
    [NonSerialized] public bool hasConnectedRoom;

    bool doorOpen = true;

    RoomManager roomManager;
    BoxCollider2D collisionCollider;
    BoxCollider2D triggerCollider;
    SpriteRenderer spriteRenderer;
    SpriteMask mask;


    private void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
        collisionCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        triggerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mask = GetComponentInChildren<SpriteMask>();

        SetDoorCollisionSize();

        roomManager.entrances.Add(this);
        
        collisionCollider.gameObject.SetActive(!doorOpen);
        UpdateSprite();
    }

    public void OnConnectedRoomSpawned()
    {
        triggerCollider.enabled = true;
        hasConnectedRoom = true;
    }

    private void SetDoorCollisionSize()
    {
        int numDir = (int)direction;
        collisionCollider.size =
            numDir == 0 || numDir == 2 ? new Vector2(3, 3) : new Vector2(1, 3);
        collisionCollider.offset =
            numDir == 0 || numDir == 2 ? new Vector2(0, -1) : Vector2.zero;
    }

    public void SpawnDoorCover()
    {
        roomManager.SpawnDoorCover(direction, transform.position);
        Invoke("Die", .1f);
    }

    public void Die()
    {
        if (gameObject != null)
            gameObject.SetActive(false);
        if (roomManager != null) roomManager.entrances.Remove(this);
            Destroy(gameObject);
    }

    public void CloseDoor()
    {
        if (doorOpen) ToggleDoor();
    }

    public void OpenDoor()
    {
        if (!doorOpen) ToggleDoor();
    }

    public Room GetRoom()
    {
        return transform.parent.GetComponent<Room>();
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
            mask.transform.localPosition = new Vector3(0, 2, 0);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        roomManager.onEntranceExit.Invoke();
    }
}
