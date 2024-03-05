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
    float vanishTimer;
    float damagePerBullet;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        gun = FindObjectOfType<GunFire>();

        vanishTimer = gun.GetVanishAfter();
        damagePerBullet = gun.GetDamagePerBullet();
    }

    private void Update()
    {
        Vanish();
    }

    void FixedUpdate()
    {
        myRigidbody.velocity = - transform.right * - gun.GetProjectileSpeed();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.transform.tag == "Enemy")
        {
            ÄckelBehavior enemyScript = other.gameObject.GetComponent<ÄckelBehavior>();

            enemyScript.TakeDamage(damagePerBullet);
        }

        var hitEffect = Instantiate(gun.GetHitEffect(), transform.position, Quaternion.identity);
        Destroy(hitEffect, gun.GetDestroyHitEffectAfter());
        Destroy(gameObject);
    }

    void Vanish()
    {
        vanishTimer -= Time.deltaTime;
        if (vanishTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
