using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowKey : MonoBehaviour
{
    public Transform target; // The target GameObject to follow
    public float followSpeed = 5f; // The speed at which this GameObject follows the target
    public float maxDistance = 10f; // The maximum distance between this GameObject and the target

    void Update()
    {
        if (target != null)
        {
            // Calculate the direction from this GameObject to the target
            Vector3 direction = target.position - transform.position;

            // Calculate the distance between this GameObject and the target
            float distance = direction.magnitude;

            // If the distance is greater than the maximum distance, move towards the target
            if (distance > maxDistance)
            {
                // Calculate the desired position to move towards
                Vector3 desiredPosition = target.position - direction.normalized * maxDistance;

                // Move towards the desired position
                transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            }
        }
    }

}
