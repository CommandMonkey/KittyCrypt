using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingBullet : Bullet
{
    Bullet bullet;
    bool inWall = false;

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
            Debug.Log("crossbowhit");
            enemyScript.TakeDamage(damage);
        }

        var ffxInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(ffxInstance, destroyHitEffectAfter);
    }
}
