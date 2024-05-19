using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkullEnemy : Enemy
{
    [Header("Stats")]
    [SerializeField] float movementSpeed;
    [SerializeField] float shootingTime;
    [SerializeField] float walkingTime;
    [Header("")]

    [Header("Bings zone")]
    [SerializeField] GameObject enemyFlamePrefab;
    [SerializeField] GameObject hitEffect;
    [SerializeField] float bulletSpeed;
    [SerializeField] int damage;

    [Header("")]
    [SerializeField] bool isShooting;
    [SerializeField] bool ableToShoot;

   

//The Attacking Fuctions ---------------------------------

    void MeleeAttack()
    {

    }

    void Shoot()
    {
        Vector2 direction = targetPosition - transform.position;

        // Räkna ut vinkeln i radianer och konvertera till grader
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Skapa en quaternion från denna vinkel
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        GameObject EnemyFlame = Instantiate(enemyFlamePrefab, transform.position, rotation);
        EnemyFlame.GetComponent<EnemyBullet>(). Initialize(bulletSpeed, damage, hitEffect);
        
    }

//The Attacking ------------------------------------------

    void Update()
    {
        ShootLineOfSightRay();

        if (lineOfSight)
        {
            if (ableToShoot)
            {
                StartCoroutine(ShootingAndScooting());
            }
            if (!isShooting) 
            {
                MoveTowardsTarget();
            }
        }
    }

    IEnumerator ShootingAndScooting()
    {
        ableToShoot = false;
        isShooting = true;
        Shoot();
        yield return new WaitForSeconds(shootingTime);
        isShooting = false;
        yield return new WaitForSeconds(walkingTime);
        ableToShoot = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        MeleeAttack();
    }

//The Scooting ------------------------------------------

    void MoveTowardsTarget()
    {
        if (targetPosition == Vector3.zero) return;
        // Calculate the direction from the current position to the target position
        Vector3 direction = targetPosition - transform.position;

        // Normalize the direction vector to ensure consistent speed in all directions
        direction.Normalize();

        //Move myself
        rigidBody2D.velocity = direction * movementSpeed;
    }
}
