using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float vanishTimer = 3f;

    Rigidbody2D myRigidbody;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        vanishTimer -= Time.deltaTime;
        if (vanishTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        myRigidbody.velocity = transform.up * -moveSpeed;
    }
}
