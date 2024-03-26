using System;
using System.Collections.Generic;
using UnityEngine;


public class Entrance : MonoBehaviour
{
    public Direction direction;
    [SerializeField] Sprite frontClosedDoorSprite;
    [SerializeField] Sprite frontOpenDoorSprite;
    [SerializeField] Sprite sideClosedDoorSprite;
    [SerializeField] Sprite sideOpenDoorSprite;

    [NonSerialized] public GameObject roomToSpawn;
    [NonSerialized] public List<string> roomTriesNames = new List<string>();
     public bool hasConnectedRoom;

    [SerializeField] bool doorOpen = true;

    RoomManager roomManager;
    BoxCollider2D doorBoxCollision;
    SpriteRenderer spriteRenderer;
    //Animator animator;

    private void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
        doorBoxCollision = GetComponentInChildren<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();

        //animator.SetInteger("direction", (int)direction);

        SetDoorCollisionSize();

        roomManager.entrances.Add(this);

        doorBoxCollision.gameObject.SetActive(!doorOpen);
        //animator.SetBool("doorOpen", doorOpen);
        UpdateSprite();

    }

    private void SetDoorCollisionSize()
    {
        int numDir = (int)direction;
        doorBoxCollision.size =
            numDir == 0 || numDir == 2 ? new Vector2(3, 1) : new Vector2(1, 3);
    }

    public void SpawnDoorCover()
    {
        roomManager.SpawnDoorCover(direction, transform.position);
        Invoke("Die", .1f);
    }

    public void Die()
    {
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

        doorBoxCollision.gameObject.SetActive(!doorOpen);
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

}
