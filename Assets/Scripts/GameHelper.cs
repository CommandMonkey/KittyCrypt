using System;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GameHelper
{
    public static Vector2 GetRandomPosInCollider(BoxCollider2D collider)
    {
        Vector2 pos = collider.transform.position - (Vector3)(collider.size / 2) + (Vector3)collider.offset;
        pos.x = UnityEngine.Random.Range(pos.x, collider.size.x);
        pos.y = UnityEngine.Random.Range(pos.y, collider.size.y);
        return pos;
    }
}