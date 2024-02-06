using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class EnemyBehavior : MonoBehaviour
{

    //Me and the targets position  

    public Transform target; 

    //Public variables
    
    [SerializeField] public float speed = 5f;
    [SerializeField] public float aiShootingDistance = 5f;


    //Private varibles

    private bool lineOfSight = true;
    private bool shootingDistance = false;

    //LayerMasks

    public LayerMask playerLayer;
    public LayerMask obsticleLayer;

    //Components

    void Start()
    {
        //gonna keep this here for later 
    }


    void Update()
    {
        if (lineOfSight == true & shootingDistance == false)
        {
            MoveTowardsTarget();
        }
        
        ShootLineOfSightRay();

        
    }

    void ShootLineOfSightRay()
    {
        Vector2 direction = target.position - transform.position;
        float distance = direction.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance,obsticleLayer);

        Debug.DrawLine(transform.position, hit.point, Color.white, 2.5f);
        if (hit.collider != null)
        {
            lineOfSight = false;
        }
        else
        {
            lineOfSight = true;
        }
    }

    void ShootDistanceRay()
    {

        RaycastHit2D hit = Physics2D.Raycast(target.position, transform.position, aiShootingDistance, playerLayer);

        if (hit.collider != null)
        {
            shootingDistance = false;
        }
        else
        {
            shootingDistance = true;
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
