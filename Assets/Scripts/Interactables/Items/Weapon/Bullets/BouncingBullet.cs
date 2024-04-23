using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBullet : Bullet
{
    float destroyAfter = 2;
    private void Start()
    {
        Invoke("Explode", destroyAfter);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Enemy enemyScript = other.gameObject.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.TakeDamage(damage);
            Explode();
        }
    }

    void Explode()
    {
        var ffxInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(ffxInstance, destroyHitEffectAfter);
        Destroy(gameObject);
    }
}
