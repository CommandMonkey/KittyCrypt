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
    Player player;

    private void Awake()
    {
        onPlayerEnter = new UnityEvent();
    }

    void Start()
    {
        player = FindObjectOfType<Player>();

        entrances = new List<Entrance>(FindObjectsByType<Entrance>(FindObjectsSortMode.None));

        // subscribe to entrances exit events
        foreach (Entrance entr in entrances)
        {
            Debug.Log(entr.name);
            entr.onEntranceExit.AddListener(OnPlayerLeftEntrance);
        }

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
