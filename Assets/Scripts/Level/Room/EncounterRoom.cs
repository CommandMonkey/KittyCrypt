using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterRoom : Room
{
    [SerializeField] List<GameObject> enemiesToSpawn;
    [SerializeField] LayerMask noEnemyLayers;
    //[SerializeField] GameObject spawnAnimPrefab;

    bool isActive = false; // if the player has entered and the encounter is active
    bool isRoomDefeated = false; // if the room has been defeted
    List<GameObject> enemies;

    RoomManager thisRoomManager;
    BoxCollider2D roomCollider;
    LevelManager levelManager;


    // Start is called before the first frame update
    protected override void RoomStart()
    {
        thisRoomManager = FindObjectOfType<RoomManager>();
        roomCollider = GetComponent<BoxCollider2D>();
        levelManager = FindObjectOfType<LevelManager>();

        base.onPlayerEnter.AddListener(OnPlayerEnter);
        levelManager.onEnemyKill.AddListener(OnEnemyKill);
    }

    private void Update()
    {

    }

    void OnEnemyKill()
    {
        if (!isActive || isRoomDefeated) return;
        int _enemiesAlive = enemies.Count-1;
        foreach(GameObject obj in enemies)
        {
            if (obj == null) _enemiesAlive--; 
        }
        Debug.Log(_enemiesAlive);
        if (_enemiesAlive == 0)
        {
            isRoomDefeated = true;
            isActive = false;
            thisRoomManager.OpenDoors();
        }
    }


    void OnPlayerEnter()
    {
        if (isRoomDefeated || isActive) return;

        isActive = true;
        thisRoomManager.CloseDoors();
        enemies = SpawnEnemies(roomCollider, enemiesToSpawn, noEnemyLayers);
    }


    public static List<GameObject> SpawnEnemies(BoxCollider2D roomCollider, List<GameObject> enemiesToSpawn, LayerMask noEnemyLayer)
    {
        List<GameObject> enemies = new List<GameObject>();

        for(int i = 0; i < enemiesToSpawn.Count; i++)
        { 
            Vector2 pos = GameHelper.GetRandomPosInCollider(roomCollider, noEnemyLayer);

            enemies.Add( Instantiate(enemiesToSpawn[i], pos, Quaternion.identity));
        }
        return enemies;
    }
}
