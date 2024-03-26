using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GameHelper
{
    public static Vector2 GetRandomPosInCollider(BoxCollider2D collider)
    {
        Vector2 pos = (Vector2)collider.transform.position - (collider.size / 2) + collider.offset;

        // Adjust position by half the size of the collider to ensure the position is within the bounds
        pos.x += UnityEngine.Random.Range(0f, collider.size.x);
        pos.y += UnityEngine.Random.Range(0f, collider.size.y);

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
}