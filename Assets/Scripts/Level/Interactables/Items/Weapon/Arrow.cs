using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Bullet bullet;
    private float damage;
    private GameObject hitEffect;
    private float destroyHitEffectAfter;

    //Cached references
    Rigidbody2D myRigidbody;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        bullet = GetComponent<Bullet>();
        damage = bullet.GetDamage();
        hitEffect = bullet.GetHitEffect();
        destroyHitEffectAfter = bullet.GetDestroyHitEffectAfter();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Entrance"))
        {
            Debug.Log("HITTING WALL");
            myRigidbody.velocity = Vector3.zero;
            { return; }
        }

        Enemy enemyScript = other.gameObject.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.GetComponent<Rigidbody2D>().velocity += -(Vector2)transform.forward * 100;
            enemyScript.TakeDamage(damage);
        }

        var ffxInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(ffxInstance, destroyHitEffectAfter);
    }
}
