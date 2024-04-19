using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBullet : Bullet
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        Enemy enemyScript = other.gameObject.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            Debug.Log("BOING");
            enemyScript.TakeDamage(damage);

            var ffxInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(ffxInstance, destroyHitEffectAfter);
            Destroy(gameObject);
        }
    }
}
