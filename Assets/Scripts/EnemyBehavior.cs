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
    public Transform myself;
    public Transform target; 
    public float speed = 5f;

    private bool lineOfSight = true;

    void Update()
    {
        if (lineOfSight == true)
        {
            MoveTowardsTarget();

            ShootRaycast();
            
        }
       
    }

    void ShootRaycast()
    {
        RaycastHit2D hit = Physics2D.Raycast(target.position, myself.position);


        if (hit.collider != null)
        {
            lineOfSight = true;
        }
        else
        {
            lineOfSight = false;
        }

    }

    void MoveTowardsTarget()
    {
        // Calculate the direction from the current position to the target position
        Vector3 direction = target.position - transform.position;

        // Normalize the direction vector to ensure consistent speed in all directions
        direction.Normalize();

        // Move the object towards the target using Translate
        transform.Translate(direction * speed * Time.deltaTime);
    }




}
