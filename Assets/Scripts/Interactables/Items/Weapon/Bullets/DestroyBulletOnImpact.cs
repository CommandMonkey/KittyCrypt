using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class DestroyBulletOnImpact : Bullet
{
    private void OnTriggerEnter2D(Collider2D other)
    { 
        Enemy enemyScript = other.gameObject.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.TakeDamage(damage);
        }

        var ffxInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(ffxInstance, destroyHitEffectAfter);
        Destroy(gameObject);
    }
}
