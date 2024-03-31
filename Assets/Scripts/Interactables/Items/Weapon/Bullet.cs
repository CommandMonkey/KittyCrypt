using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Configurable parameters

    //Cached references
    Rigidbody2D myRigidbody;
    GunFire gun;

    //Private variables

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        gun = FindObjectOfType<GunFire>();
        Vanish();
    }

    void FixedUpdate()
    {
        myRigidbody.velocity = transform.right * gun.GetProjectileSpeed();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.transform.tag == "Enemy")
        {
            RushingEnemyBehavior enemyScript = other.gameObject.GetComponent<RushingEnemyBehavior>();

            enemyScript.gameObject.GetComponent<Rigidbody2D>().velocity += -(Vector2)transform.forward * 100;
            enemyScript.TakeDamage(gun.GetDamagePerBullet());
        }

        var hitEffect = Instantiate(gun.GetHitEffect(), transform.position, Quaternion.identity);
        Destroy(hitEffect, gun.GetDestroyHitEffectAfter());
        Destroy(gameObject);
    }

    IEnumerator Vanish()
    {
        yield return new WaitForSeconds(gun.GetVanishAfter());
        Destroy(gameObject);
    }
}
