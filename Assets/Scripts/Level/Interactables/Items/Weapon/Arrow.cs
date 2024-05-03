using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Bullet bullet;
    private float damage;
    private GameObject hitEffect;
    private float destroyHitEffectAfter;
    bool inWall = false;

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
        if (other.gameObject.CompareTag("Wall"))
        {
            myRigidbody.velocity = Vector3.zero;
            inWall = true;
        }

        if(inWall) { return; }

        Enemy enemyScript = other.gameObject.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.TakeDamage(damage);
        }

        var ffxInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(ffxInstance, destroyHitEffectAfter);
    }
}
