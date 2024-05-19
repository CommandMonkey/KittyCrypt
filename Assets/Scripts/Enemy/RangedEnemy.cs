using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RangedEnemy : Enemy
{
    [Header("Guns")]
    [SerializeField] private GunProperties[] guns;
    [SerializeField] private GameObject bullethitVFX;
    [SerializeField] private float gunsChargeUpTime = 0.5f;
    [SerializeField] private float GunsCooldownTime = 2f;
    [SerializeField] private float GunsShootingDuration = 3f;
    [SerializeField] private float maxShootingDistance = 5f; 
    //[Header("Gun Rotation")]
    //[SerializeField] private float gunRotationSpeed;
    //[SerializeField] private float shootingGunRotationSpeed;
    //[SerializeField] private float maxRotationSpeed;
    //[SerializeField] private float maxAngularVelocity;
    [SerializeField] private float pointingThresholdAngle = 1f;

    [System.Serializable] private class GunProperties
    {
        public Transform gunTransform;
        public float gunCooldownTime = 1f;
        public GameObject bulletPrefab;
        public float bulletSpeed = 2f;
        public int bulletDamage = 1;
        public float bulletSpreadRange = 1f;

        private float curentCooldown = 0;
        private float onCooldownStartTime;

        public void StartCoolDown() { onCooldownStartTime = Time.time; }
        public bool isOnCoolDown() { return Time.time - onCooldownStartTime <= gunCooldownTime; }
        
    }
    enum gunState
    {
        shooting,
        cooldown,
        idle,
    }
    [SerializeField] gunState state = gunState.idle;

    [SerializeField] bool canSeeTarget;


    // Update is called once per frame
    void Update()
    {
        if (state != gunState.cooldown)
        {
            canSeeTarget = ShootLineOfSightRay();


            if (canSeeTarget && IsWithinShootingDistance())
            {
                StopMoving();
                if (state == gunState.idle && IsGunsPointingTowardsTarget())
                {
                    Debug.Log("STARTING SHOOTING");
                    state = gunState.cooldown;
                    Invoke("StartShooting", gunsChargeUpTime);
                }
            }
            else 
                MoveTowardsTarget();
        }


        SetGunsFacings();
    }

    private void PauseGunRotation()
    {
        foreach (var gun in guns)
        {
            gun.gunTransform.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        }
    }

    private void StopMoving()
    {
        rigidBody2D.velocity = Vector3.zero;
        animator.SetBool("isMoving", false);
    }

    void MoveTowardsTarget()
    {   
        animator.SetBool("isMoving", true);
        if (targetPosition == Vector3.zero) return;
            
        // Calculate the direction from the current position to the target position
        Vector3 direction = (targetPosition - transform.position).normalized;

        //Move myself
        rigidBody2D.velocity = direction * speed;

    }


    void StartShooting()
    {
        if (!canSeeTarget) return;
        state = gunState.shooting;
        StartCoroutine(ShootingRoutine());
    }



    IEnumerator ShootingRoutine()
    {
        Debug.Log("shooting routine start");

        float TimeElapsed = 0f;
        while (TimeElapsed <= GunsShootingDuration)
        {
            Debug.Log("ShootTick");
            foreach (GunProperties gun in guns)
            {
                if (!gun.isOnCoolDown())
                {
                    gun.StartCoolDown();
                    shoot(gun);
                }
            }
            yield return new WaitForFixedUpdate();
            TimeElapsed += Time.deltaTime;
        } 



        StopShooting();
    }

    private void shoot(GunProperties gun)
    {
        EnemyBullet bullet = Instantiate(gun.bulletPrefab, gun.gunTransform.position, GetBulletSpread(gun)).GetComponent<EnemyBullet>();
        bullet.Initialize(gun.bulletSpeed, gun.bulletDamage, bullethitVFX);
    }

    void StopShooting()
    {
        state = gunState.cooldown;
        Invoke("StopCoolDown", GunsCooldownTime);
    }

    void StopCoolDown()
    {
        state = gunState.idle;
    }

    void SetGunsFacings()
    {
        foreach (GunProperties gun in guns)
        {
            SetGunDirection(gun.gunTransform);
        }
    }

    private void SetGunDirection(Transform gunTransform)
    {
        // Get the direction between weaponTransform and targetPos
        Vector2 targetDirection = (targetPosition - gunTransform.position).normalized;

        // Calculate the angle between the current forward direction and the target direction
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        gunTransform.rotation = rotation;

        //// Calculate the difference in angle between current rotation and target angle
        //float angleDifference = Mathf.DeltaAngle(gunTransform.eulerAngles.z, targetAngle);

        //// Calculate the desired rotation speed based on the angle difference
        //float desiredRotationSpeed = Mathf.Clamp(angleDifference * rotationAcceleration, -maxRotationSpeed, maxRotationSpeed);

        //// Apply rotation speed to the weapon
        //float newRotationSpeed = Mathf.MoveTowards(gunTransform.GetComponent<Rigidbody2D>().angularVelocity, desiredRotationSpeed, Time.deltaTime * rotationAcceleration);
        //newRotationSpeed = Mathf.Clamp(newRotationSpeed, -maxAngularVelocity, maxAngularVelocity);
        //gunTransform.GetComponent<Rigidbody2D>().angularVelocity = newRotationSpeed;
    }

    bool IsGunsPointingTowardsTarget()
    {
        bool result = true;
        foreach (GunProperties gun in guns)
        {
            if (!IsPointingTowardsTarget(gun.gunTransform))
            {
                result = false;
                break;
            }
        }
        return result;
    }
    bool IsPointingTowardsTarget(Transform gun)
    {
        Vector3 directionToTarget = (targetPosition - gun.position).normalized;
        float angle = Vector3.Angle(directionToTarget, gun.right);
        return angle <= pointingThresholdAngle;
    }

    private bool IsWithinShootingDistance()
    {
        return (targetPosition - transform.position).magnitude <= maxShootingDistance;
    }


    private Quaternion GetBulletSpread(GunProperties gun)
    {
        Vector3 angles = gun.gunTransform.rotation.eulerAngles;
        Vector3 newAngles = new Vector3(
            angles.x,
            angles.y,
            angles.z + UnityEngine.Random.Range(-gun.bulletSpreadRange, gun.bulletSpreadRange));
        return Quaternion.Euler(newAngles);
    }
}
