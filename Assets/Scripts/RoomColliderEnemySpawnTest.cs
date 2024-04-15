using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomColliderEnemySpawnTest : MonoBehaviour
{
    [SerializeField] LayerMask obsticleMask;
    [SerializeField] int amountOfTries = 100;
    [SerializeField] GameObject ToSpawn;
    BoxCollider2D roomCol;
    // Start is called before the first frame update
    void Start()
    {
        roomCol = GetComponent<BoxCollider2D>();

        StartCoroutine(SpawnStuff());


    }

    IEnumerator SpawnStuff()
    {
        for (int i = 0; i < amountOfTries; i++)
        {
            Debug.Log("Spawning thing");
            Vector2 pos = GameHelper.GetRandomPosInCollider(roomCol, obsticleMask);
            Instantiate(ToSpawn, pos, Quaternion.identity);
            yield return new WaitForFixedUpdate();
        }
    }


}
