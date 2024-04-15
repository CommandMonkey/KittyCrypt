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
        myRigidbody.velocity = transform.right * speed;
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
    }

    public float GetDamage()
    {
        return damage;
    }

    public GameObject GetHitEffect()
    {
        return hitEffect;
    }

    public float GetDestroyHitEffectAfter()
    {
        return destroyHitEffectAfter;
    }
}
