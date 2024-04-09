using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RushingEnemyBehavior : Enemy
{

    //Private Variables
    private bool IsMeleeOnCooldown = true;
    private Vector3 previousPosition;

    // called from baseClass
    protected override void EnemyStart()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        MoveTowardsTarget(); 
        animator.SetBool("isRunning", true); 

        CheckWalkDirection();
        ShootLineOfSightRay();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.transform.tag == "Player")
        {
            HitPlayer();
        }
    }

    void HitPlayer()
    {
        animator.SetTrigger("isAttacking");
        if (!IsMeleeOnCooldown) return;
        player.TakeDamage(enemyDMG);
        
        StartCoroutine(MeleeCooldownRoutine());
    }



    IEnumerator MeleeCooldownRoutine()
    {
        IsMeleeOnCooldown = false;
        yield return new WaitForSeconds(attackSpeed);
        IsMeleeOnCooldown = true;
    }

    void CheckWalkDirection()
    {
        Vector3 currentPosition = transform.position;

        float deltaX = currentPosition.x - previousPosition.x;
        if (deltaX > 0)
        { transform.rotation = Quaternion.Euler(0f, 180f, 0f); }
        else if (deltaX < 0)
        { transform.rotation = Quaternion.Euler(0f, 0f, 0f); }

        previousPosition = currentPosition;
    }
 

    void MoveTowardsTarget()
    {

        if (targetPosition == Vector3.zero) return;
        // Calculate the direction from the current position to the target position
        Vector3 direction = targetPosition - transform.position;

        // Normalize the direction vector to ensure consistent speed in all directions
        direction.Normalize();

        //Move myself
        rigidBody2D.velocity = direction * speed;


    }

}