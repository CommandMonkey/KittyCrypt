using System;
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
}