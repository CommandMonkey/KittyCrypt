using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class DestroyOnImpact : MonoBehaviour
{
    Bullet bullet;
    private float damage;
    private GameObject hitEffect;
    private float destroyHitEffectAfter;

    private void Start()
    {
        bullet = GetComponent<Bullet>();
        damage = bullet.GetDamage();
        hitEffect = bullet.GetHitEffect();
        destroyHitEffectAfter = bullet.GetDestroyHitEffectAfter();
    }

    private void OnTriggerEnter2D(Collider2D other)
    { 
        Enemy enemyScript = other.gameObject.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.GetComponent<Rigidbody2D>().velocity += -(Vector2)transform.right * 100;
            enemyScript.TakeDamage(damage);
        }

        var ffxInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(ffxInstance, destroyHitEffectAfter);
        Destroy(gameObject);
    }
}
