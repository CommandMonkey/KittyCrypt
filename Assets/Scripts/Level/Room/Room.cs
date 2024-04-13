using System;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Direction
{
    Bottom,
    Left,
    Top, 
    Right
}


public class Room : MonoBehaviour
{
    public List<Entrance> entrances;

    [NonSerialized] public Entrance previousRoomEntrance;
    [NonSerialized] protected UnityEvent onPlayerEnter;

    // Cached references
    protected Player player;
    GameObject fogOfWarObject;
    protected RoomManager roomManager;

    private void Awake()
    {
        onPlayerEnter = new UnityEvent();
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        fogOfWarObject = GameHelper.GetChildWithTag(gameObject, "FogOfWar");
        roomManager = FindObjectOfType<RoomManager>();

        entrances = transform.GetComponentsInAllChildren<Entrance>();

        roomManager.onEntranceExit.AddListener(OnPlayerLeftEntrance);

        RoomStart();
    }

    private void Die()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    void OnPlayerLeftEntrance()
    {
        if (IsPlayerInside())
        {
            if (fogOfWarObject != null)
            {
                fogOfWarObject.GetComponent<Animation>().Play();
                Destroy(fogOfWarObject, 2f);
            }

            onPlayerEnter.Invoke();
        }
    }

    protected bool IsPlayerInside()
    {
        if (player.IsOverlapping<Room>(gameObject))
        {
            return true;
        }
        return false;
    }

    protected virtual void RoomStart() { }

}
