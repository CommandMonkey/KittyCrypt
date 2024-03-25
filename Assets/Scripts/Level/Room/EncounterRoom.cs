using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterRoom : Room
{
    [SerializeField] List<GameObject> enemiesToSpawn;
    [SerializeField] LayerMask noEnemyLayers;

    bool isActive; // if the player has entered and the encounter is active
    bool isRoomDefeated; // if the room has been defeted
    List<GameObject> enemies = new List<GameObject>();

    RoomManager thisRoomManager;
    BoxCollider2D roomCollider;
    LevelManager levelManager;
    // Start is called before the first frame update
    void Start()
    {
        thisRoomManager = FindObjectOfType<RoomManager>();
        roomCollider = GetComponent<BoxCollider2D>();
        levelManager = FindObjectOfType<LevelManager>();
        OnPlayerEnter.AddListener(PlayerEnter);
    }

    private void Update()
    {
        if(isActive)
        {
            if (enemies.Count <= 0)
            {
                thisRoomManager.OpenDoors();
                isActive = false;
            }
        }
    }


    void PlayerEnter()
    {
        if (isRoomDefeated) return;
        
        StartCoroutine(PlayerEnterRoutine());
    }

    IEnumerator PlayerEnterRoutine()
    {
        yield return new WaitForSeconds(1f);
        thisRoomManager.CloseDoors();
        SpawnEnemies();
        isActive = true;
    }

    void SpawnEnemies()
    {
        enemies.Clear();

        foreach (GameObject enemy in enemiesToSpawn)
        {

            Vector2 pos = GameHelper.GetRandomPosInCollider(roomCollider);

            // get a random position
            while (Physics2D.OverlapCircle(pos, .5f, noEnemyLayers))
            {
                pos = GameHelper.GetRandomPosInCollider(roomCollider);
            }

            Debug.Log(enemy.name + " is spawning at: " + pos);
            enemies.Add(Instantiate(enemy, pos, Quaternion.identity, levelManager.enemyContainer));
        }
    }
}
