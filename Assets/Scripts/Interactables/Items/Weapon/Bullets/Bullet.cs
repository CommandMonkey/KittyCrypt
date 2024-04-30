using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Private variables
    protected float speed;
    protected float damage;
    protected GameObject hitEffect;
    protected float destroyHitEffectAfter;
    protected float vanishAfter;

    //Cached references
    protected Rigidbody2D myRigidbody;

    public void Initialize(float speed, float damage, GameObject hitEffect, float destroyHitEffectAfter = 2f, float vanishAfter = 4f)
    {
        this.speed = speed;
        this.damage = damage;
        this.hitEffect = hitEffect;
        this.destroyHitEffectAfter = destroyHitEffectAfter;
        this.vanishAfter = vanishAfter;
    }
    protected void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        Destroy(gameObject, vanishAfter);
        myRigidbody.velocity = transform.right * speed;
    }
}