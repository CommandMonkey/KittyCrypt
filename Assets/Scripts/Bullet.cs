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

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        gun = FindObjectOfType<GunFire>();

        vanishTimer = gun.GetVanishAfter();
    }

    private void Update()
    {
        Vanish();
    }

    void FixedUpdate()
    {
        myRigidbody.velocity = transform.up * - gun.GetProjectileSpeed();
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