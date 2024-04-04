using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushingEnemyBehavior : MonoBehaviour, IEnemy
{

    //variables
    [Header("Enemy stats")]
    [SerializeField, Range(0, 10)] private float speed = 5f;
    [SerializeField, Range(0, 10)] private float distanceToTarget = 6f;
    [SerializeField, Range(0, 10)] private float meleeRange = 5f;
    [SerializeField, Range(0, 1000)] private float hp = 1f;
    [SerializeField, Range(0, 100)] private int enemyDMG = 1;
    [Header("The Lower the number the faster the attack")]
    [SerializeField, Range(0.1f, 3)] private float attackSpeed = 0.1f;
    [Header("VFX")]
    [SerializeField] GameObject BloodSplatVFX;
    [SerializeField] GameObject BloodStainVFX;
    [Header("SFX")]
    [SerializeField] AudioClip hitSound;

    private Transform target;
    [SerializeField] GameObject enemyHitAudio;

    //variblues

    private bool lineOfSight = true;
    private bool shootingDistance = false;
    private bool inMeleeRange = false;
    private bool IsMeleeOnCooldown = true;

    private Vector3 playerPosition = Vector3.zero;

    private Vector3 previousPosition;

    //LayerMasks
    public LayerMask obsticleLayer;

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
        if (!shootingDistance)
        { 
            MoveTowardsTarget(); 
            animator.SetBool("isRunning", true); 
        }

        if (IsMeleeOnCooldown && inMeleeRange) 
        {
            HitPlayer();
        }

        if (lineOfSight) 
            HowFarFromTarget();

        CheckWalkDirection();
        ShootLineOfSightRay();
        CheckMeleeRange();
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
            levelManager.onEnemyKill.Invoke();
            PlayDeathVFX();
            Destroy(gameObject);
        }
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

    void CheckMeleeRange()
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
            animator.SetBool("isRunning", false);
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

        if (playerPosition == Vector3.zero) return;
        // Calculate the direction from the current position to the target position
        Vector3 direction = playerPosition - base.transform.position;

        // Normalize the direction vector to ensure consistent speed in all directions
        direction.Normalize();

        //Move myself
        rigidBody2D.velocity = direction * speed;


    }

}