using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RushingEnemyBehavior : Enemy
{

    //variables


    //variblues
    private bool IsMeleeOnCooldown = true;

    private Vector3 previousPosition;

    //LayerMasks

    //declerations
    LevelManager levelManager;
    Rigidbody2D rigidBody2D;
    Player player;
    Animator animator;
    

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        rigidBody2D = GetComponent<Rigidbody2D>();

        previousPosition = transform.position;

        target = levelManager.player.transform;

        player = target.GetComponent<Player>();

        animator = GetComponentInChildren<Animator>();
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

    public void TakeDamage(float damage)
    {
        PlayHurtVFX();
        PlayHurtSFX();
        hp -= damage;
        if (hp < 0)
        {
            Die();
        }
    }

    void Die()
    {
        levelManager.onEnemyKill.Invoke();
        PlayDeathVFX();
        Destroy(gameObject);
    }

    void PlayHurtSFX()
    {
        Instantiate(enemyHitAudio);
    }

    private void PlayHurtVFX()
    {
        animator.SetTrigger("WasHurt");
        Instantiate(BloodStainVFX, transform.position, Quaternion.identity);
    }

    void PlayDeathVFX()
    {
        GameObject Blood = Instantiate(BloodSplatVFX, transform.position, transform.rotation);
        Destroy(Blood, 1f);
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

        if (playerPosition == Vector3.zero) return;
        // Calculate the direction from the current position to the target position
        Vector3 direction = playerPosition - base.transform.position;

        // Normalize the direction vector to ensure consistent speed in all directions
        direction.Normalize();

        //Move myself
        rigidBody2D.velocity = direction * speed;


    }

}