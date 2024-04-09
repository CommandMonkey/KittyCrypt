using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private float damage;
    private GameObject hitEffect;
    private float destroyHitEffectAfter;
    private float vanishAfter;



    //Cached references
    Rigidbody2D myRigidbody;

    //Private variables

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        Destroy(gameObject, vanishAfter);
    }
    public void Initialize(float speed, float damage, GameObject hitEffect, float destroyHitEffectAfter = 2f, float vanishAfter = 4f)
    {
        this.speed = speed;
        this.damage = damage;
        this.hitEffect = hitEffect;
        this.destroyHitEffectAfter = destroyHitEffectAfter;
        this.vanishAfter = vanishAfter;
    }

    void FixedUpdate()
    {
        myRigidbody.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemyScript = other.gameObject.GetComponent<Enemy>();

            enemyScript.GetComponent<Rigidbody2D>().velocity += -(Vector2)transform.forward * 100;
            enemyScript.TakeDamage(damage);
        }

        var ffxInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(ffxInstance, destroyHitEffectAfter);
        Destroy(gameObject);
    }


}
