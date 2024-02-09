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

    //Public variables
    
    [SerializeField, Range(1, 10)] public float speed = 5f;
    [SerializeField, Range(2, 10)] public float distanceToTarget = 5f;

    //Private varibles

    private bool lineOfSight = true;
    private bool shootingDistance = false;

    //LayerMasks

    public LayerMask playerLayer;
    public LayerMask obsticleLayer;

    //Components

    void Update()
    {
        if (lineOfSight == true & shootingDistance == false)
        {
            MoveTowardsTarget();
        }
        ShootLineOfSightRay();

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= distanceToTarget)
        {
            shootingDistance = true;
        }
        else
        {
            shootingDistance = false;
        }
    }

    void ShootLineOfSightRay()
    {
        Vector2 direction = target.position - transform.position;
        float distance = direction.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obsticleLayer);

        Debug.DrawLine(transform.position, hit.point, Color.white, 0.1f);

        if (hit.collider != null)
        {
            lineOfSight = false;
        }
        else
        {
            lineOfSight = true;
        }
    }

    void MoveTowardsTarget()
    {
        // Calculate the direction from the current position to the target position
        Vector3 direction = target.position - base.transform.position;

        // Normalize the direction vector to ensure consistent speed in all directions
        direction.Normalize();

        // Move the object towards the target using Translate
        base.transform.Translate(direction * speed * Time.deltaTime);
    }
}
