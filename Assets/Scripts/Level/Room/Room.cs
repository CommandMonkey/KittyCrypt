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
    protected GameSession gameSession;
    protected Player player;
    protected RoomManager roomManager;
    FogOfWar fogOfWarObject;

    private void Awake()
    {
        onPlayerEnter = new UnityEvent();
    }

    void Start()
    {
        gameSession = GameSession.Instance;
        player = gameSession.player;
        roomManager = FindObjectOfType<RoomManager>();
        fogOfWarObject = GetComponentInChildren<FogOfWar>();

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
                fogOfWarObject.FadeAway();
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
