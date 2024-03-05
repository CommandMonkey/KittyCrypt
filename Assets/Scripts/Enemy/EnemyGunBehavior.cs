using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyGunBehavior : MonoBehaviour
{
    //Declarations
    ÄckelBehavior enemyBehavior;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        enemyBehavior = GetComponentInParent<ÄckelBehavior>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //aimToPlayer();
    }

    void aimToPlayer()
    {
        //Point to player 
        //Vector3 directionToTarget = enemyBehavior.target.position - transform.position;

        //float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        //transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        ////Flip if upside down
        //if (transform.rotation.eulerAngles.z is >= 90 and <= 270)
        //{ spriteRenderer.flipY = true; }
        //else
        //{ spriteRenderer.flipY = false; }
    }

    void shoot()
    { 
        
    }
}