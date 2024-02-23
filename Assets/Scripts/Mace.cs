using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mace : MonoBehaviour
{
    private enemy_Test enemyT;

    public Transform circleOrgin;
    public float radius;


    private void Start()
    {
        enemyT = FindObjectOfType<enemy_Test>();
    }

    private void Update()
    { 
        if (Input.GetMouseButtonDown(0))
        {
            DetectColliders();
        }
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = circleOrgin == null ? Vector3.zero : circleOrgin.position;
        Gizmos.DrawWireSphere(position, radius);
    }


    public void DetectColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrgin.position, radius))
        {
            enemyT.TakeDamage();
            Debug.Log("Hit");
        }
    }
}


