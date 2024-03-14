using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatBehavior : MonoBehaviour
{
    // is un used 

    //variables
    [SerializeField, Range(1, 10)] private float speed = 5f;
    [SerializeField, Range(1, 10)] private float distanceToTarget = 5f;
    [SerializeField, Range(1, 10)] private float meleeRange = 5f;
    [SerializeField, Range(1, 1000)] private float HP = 1f;

    private Transform target;

    //varibles
    private bool lineOfSight = true;
    private bool shootingDistance = false;
    private bool inMeleeRange = false;

    private Vector3 playerPosition = Vector3.zero;

    private Vector3 previousPosition;

    //LayerMasks
    public LayerMask obsticleLayer;

    //declerations
    LevelManager gameManager;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidBody2D;

    void Start()
    {
        gameManager = FindObjectOfType<LevelManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody2D = GetComponent<Rigidbody2D>();

        previousPosition = transform.position;
        target = gameManager.player.transform;
    }

    void Update()
    {
        if (shootingDistance == false)
        { MoveTowardsTarget(); }

        if (inMeleeRange == true)
        { HitPlayer(); }

        if (lineOfSight == true)
        { HowFarFromTarget(); }

        CheakWalkDirection();
        ShootLineOfSightRay();
        ShootMeleeRay();
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
        if (HP < 0)
        {
            Destroy(gameObject);
        }
    }

    void HitPlayer()
    {

    }

    void CheakWalkDirection()
    {
        Vector3 currentPosition = transform.position;

        float deltaX = currentPosition.x - previousPosition.x;
        if (deltaX > 0)
        { spriteRenderer.flipX = true; }
        else if (deltaX < 0)
        { spriteRenderer.flipX = false; }

        previousPosition = currentPosition;
    }

    void ShootMeleeRay()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= meleeRange)
        { inMeleeRange = true; }
        else
        { inMeleeRange = false; }
    }

    void ShootLineOfSightRay()
    {
        Vector2 direction = target.position - transform.position;
        float distance = direction.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obsticleLayer);

        Debug.DrawLine(transform.position, hit.point, Color.blue, 0.1f);

        if (hit.collider != null)
        {
            lineOfSight = false;
        }
        else
        {
            lineOfSight = true;
            playerPosition = target.position;
        }
    }

    void HowFarFromTarget()
    {
        float distance = Vector3.Distance(transform.position, target.position );

        if (distance <= distanceToTarget)
        { shootingDistance = true; }
        else
        { shootingDistance = false; }
    }

    void MoveTowardsTarget()
    {
        if (playerPosition == Vector3.zero) return;
        // Calculate the direction from the current position to the target position
        Vector3 direction = playerPosition - base.transform.position;

        // Normalize the direction vector to ensure consistent speed in all directions
        direction.Normalize();

        //Move myself
        rigidBody2D.velocity = direction * speed;
    }





}
