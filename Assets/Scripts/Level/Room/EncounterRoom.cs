using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterRoom : Room
{
    [SerializeField] int enemyValueCapacity = 30;
    [SerializeField] List<GameObject> enemiesToSpawn;
    [SerializeField] LayerMask noEnemyLayers;
    [SerializeField] GameObject healthPrefab;
    //[SerializeField] GameObject spawnAnimPrefab;

    bool isActive = false; // if the player has entered and the encounter is active
    bool isRoomDefeated = false; // if the room has been defeted
    bool enemiesSpawned = false;
    Vector3 finalEnemyPos;
    List<GameObject> enemies;

    RoomManager thisRoomManager;
    BoxCollider2D roomCollider;
    MusicManager musicManager;


    // Start is called before the first frame update
    protected override void RoomStart()
    {
        thisRoomManager = FindObjectOfType<RoomManager>();
        roomCollider = GetComponent<BoxCollider2D>();
        musicManager = FindObjectOfType<MusicManager>();

        base.onPlayerEnter.AddListener(OnPlayerEnter);
        //levelManager.onEnemyKill.AddListener(OnEnemyKill);

        enemiesToSpawn = gameSession.levelSettings.GetEnemyList(enemyValueCapacity);

        enemies = new List<GameObject>();
    }

    private void Update()
    {
        if (!isActive || isRoomDefeated) return;
        int _enemiesAlive = enemies.Count;
        foreach (GameObject obj in enemies)
        {
            if (obj == null)
                _enemiesAlive--;
            else
                finalEnemyPos = obj.transform.position;
        }
        if (_enemiesAlive == 0)
        {
            ClearRoom();
        }
    }

    void ClearRoom()
    {
        isRoomDefeated = true;
        isActive = false;
        SpawnHP_Pickups();
        thisRoomManager.OpenDoors();
        musicManager.PlayExploringTheme(false);
        gameSession.roomsCleared++;
    }

    //void OnEnemyKill()
    //{
    //    if (!isActive || isRoomDefeated) return;
    //    int _enemiesAlive = enemies.Count-1;
    //    foreach(GameObject obj in enemies)
    //    {
    //        if (obj == null) _enemiesAlive--; 
    //    }
    //    if (_enemiesAlive == 0)
    //    {
    //        isRoomDefeated = true;
    //        isActive = false;
    //        thisRoomManager.OpenDoors();
    //    }
    //}

    void SpawnHP_Pickups()
    {
        int amountOfHealth = UnityEngine.Random.Range(1, 3); 
        for (int i = 0; i < amountOfHealth; i++)
        {
            Instantiate(healthPrefab, finalEnemyPos, Quaternion.identity);
        }
    }

    void OnPlayerEnter()
    {
        if (isRoomDefeated || isActive || enemiesSpawned) return;

        enemiesSpawned = true;
        Invoke("SetActive", 3f);
        thisRoomManager.CloseDoors();
        SpawnEnemies();

        musicManager.PlayBattleTheme(false);

    }

    private void SetActive()
    {
        isActive = true;
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            Vector2 pos = GameHelper.GetRandomPosInCollider(roomCollider, noEnemyLayers);
            InstanciateAfterAnim spawner = Instantiate(enemiesToSpawn[i], pos, Quaternion.identity).GetComponent<InstanciateAfterAnim>();

            spawner.Initialize(this);
        }
    }


    public void RegisterEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }
}
