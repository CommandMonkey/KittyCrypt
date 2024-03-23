using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterRoom : Room
{
    [SerializeField] List<GameObject> enemiesToSpawn;
    [SerializeField] LayerMask noEnemyLayers = LayerMask.GetMask("Wall");

    BoxCollider2D roomCollider;
    LevelManager levelManager;
    // Start is called before the first frame update
    void Start()
    {
        roomCollider = GetComponent<BoxCollider2D>();
        levelManager = FindObjectOfType<LevelManager>();
        StartCoroutine(SpawnEnemiesRoutine());
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        foreach(GameObject enemy in enemiesToSpawn)
        {
            Vector2 pos = Vector2.zero;
            // get a random position
            while (Physics2D.OverlapCircle(pos, .5f, noEnemyLayers))
            {
                pos = transform.position - (Vector3)(roomCollider.size / 2);
                pos.x = UnityEngine.Random.Range(pos.x, roomCollider.size.x);
                pos.y = UnityEngine.Random.Range(pos.y, roomCollider.size.y);
            }

            Instantiate(enemy, pos, Quaternion.identity, levelManager.enemyContainer);
            
        }

        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
