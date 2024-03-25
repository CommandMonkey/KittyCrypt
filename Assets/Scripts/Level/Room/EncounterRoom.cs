using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterRoom : Room
{
    [SerializeField] List<GameObject> enemiesToSpawn;
    [SerializeField] LayerMask noEnemyLayers;

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


    void PlayerEnter()
    {
        StartCoroutine(PlayerEnterRoutine());
        
    }

    IEnumerator PlayerEnterRoutine()
    {
        yield return new WaitForSeconds(1f);
        thisRoomManager.CloseDoors();
        StartCoroutine(SpawnEnemiesRoutine());
    }

    IEnumerator SpawnEnemiesRoutine()
    {


        foreach (GameObject enemy in enemiesToSpawn)
        {

            Vector2 pos = GameHelper.GetRandomPosInCollider(roomCollider);

            // get a random position
            while (Physics2D.OverlapCircle(pos, .5f, noEnemyLayers))
            {
                pos = GameHelper.GetRandomPosInCollider(roomCollider);
            }

            Debug.Log(enemy.name + " is spawning at: " + pos);
            Instantiate(enemy, pos, Quaternion.identity, levelManager.enemyContainer);
        }

        yield break;
    }
}
