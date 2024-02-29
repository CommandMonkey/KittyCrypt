using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class EnemyBehavior : MonoBehaviour
{

    //Me and the targets position  

    public Transform target;

    //variables

    [SerializeField, Range(1, 10)] private float speed = 5f;
    [SerializeField, Range(1, 10)] private float distanceToTarget = 5f;
    [SerializeField, Range(1, 10)] private float meleeRange = 5f;
    [SerializeField, Range(1, 1000)] private float HP = 1f;

    //varibles

    private bool lineOfSight = true;
    private bool shootingDistance = false;
    private bool inMeleeRange = false;

    private Vector3 playerPosition = Vector3.zero;

    private Vector3 previousPosition;

    //LayerMasks

    public LayerMask obsticleLayer;

    //declerations

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidbody2D;

    void Start()
    {
        previousPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (shootingDistance == false )
        { MoveTowardsTarget(); }

        if (inMeleeRange == true)
        { HitPlayer(); }

        if (lineOfSight == true)
        { HowFarFromTarget(); }

        CheakWalkDirection();
        ShootLineOfSightRay();
        ShootMeleeRay();
        
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
        { inMeleeRange = true;  }
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
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= distanceToTarget)
        { shootingDistance = true; }
        else
        { shootingDistance = false; }
    }

    void MoveTowardsTarget()
    {
        // Calculate the direction from the current position to the target position
        Vector3 direction = playerPosition - base.transform.position;

        // Normalize the direction vector to ensure consistent speed in all directions
        direction.Normalize();

        // Move the object towards the target using Translate
        //transform.Translate(direction * speed * Time.deltaTime);



        rigidbody2D.velocity = direction * speed;
    }
}