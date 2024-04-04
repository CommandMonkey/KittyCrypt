using System;
using System.Collections.Generic;
using UnityEngine;

public class GameHelper : MonoBehaviour
{
    public static Vector2 GetRandomPosInCollider(BoxCollider2D collider, LayerMask filter = new LayerMask())
    {
        Vector2 min = collider.bounds.min;
        Vector2 max = collider.bounds.max;

        float randomX = UnityEngine.Random.Range(min.x, max.x);
        float randomY = UnityEngine.Random.Range(min.y, max.y);

        Vector2 pos = new Vector2(randomX, randomY);

        if (Physics2D.OverlapCircle(pos, 1f, filter))
            return GetRandomPosInCollider(collider, filter);
        else
            return pos;
    }

    public static bool IsBoxColliderTouching(Vector3 _pos, BoxCollider2D _collider, ContactFilter2D _filter)
    {
        Collider2D[] results = new Collider2D[10];

        int numColliders = Physics2D.OverlapBox((Vector2)_pos+_collider.offset, _collider.size, 0f, _filter, results);

        return numColliders > 0;
    }

    public static List<T> ShuffleList<T>(List<T> list)
    {
        List<T> shuffledList = new List<T>(list); // Copy the original list
        int n = shuffledList.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1); // Get a random index
            T value = shuffledList[k]; // Swap elements
            shuffledList[k] = shuffledList[n];
            shuffledList[n] = value;
        }
        return shuffledList;
    }


    public static List<GameObject> InstanciateInCollider(BoxCollider2D roomCollider, List<GameObject> toSpawn, LayerMask noEnemyLayer)
    {
        List<GameObject> instances = new List<GameObject>();

        for (int i = 0; i < toSpawn.Count; i++)
        {
            Vector2 pos = GameHelper.GetRandomPosInCollider(roomCollider, noEnemyLayer);
            instances.Add(Instantiate(toSpawn[i], pos, Quaternion.identity));
        }
        return instances;
    }
}