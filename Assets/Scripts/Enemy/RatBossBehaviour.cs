using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatBossBehaviour : MonoBehaviour
{
    [Header("General")]
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float timeBetweenAttacks = 4f;
    [SerializeField] LayerMask hitLayerMask;
    [Header("Attack - shoot")]
    [SerializeField] float shootAimTime = 3f;
    [SerializeField] GameObject aimLineVFX;
    [SerializeField] GameObject shootLineVFX;
    [SerializeField] Transform weaponTransform; // The transform of the weapon
    public float maxRotationSpeed = 5f; // The maximum rotation speed of the weapon
    public float rotationAcceleration = 10f; // The rotation acceleration towards the target direction
    public float maxAngularVelocity = 10f; // The maximum angular velocity of the weapon

    [Header("Test")]
    [SerializeField] Transform testMarker;

    enum RatState
    {
        paused,
        idle,
        attacking,
        dead
    }
    bool weaponAimActive = false;

    float timeToNextShot;
    Vector3 target;
    RaycastHit2D aimRayHit;

    RatState state = RatState.paused;

    Player player;
    LineRenderer aimLineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        aimLineRenderer = GetComponent<LineRenderer>();

        StartBoss();
    }

    public void StartBoss()
    {
        state = RatState.idle;
        StartCoroutine(BossAttackLoopRoutine());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (state == RatState.paused) return;

        target = player.transform.position;
        DetermineTargetGunDirection();
        if (weaponAimActive)
        {
            aimLineRenderer.enabled = true;
            AimLineVFX();
        }
        else
        {
            aimLineRenderer.enabled = false;
        }

    }



    IEnumerator BossAttackLoopRoutine()
    {
        while (state != RatState.dead)
        {
            // 3 shoot attacks then one rat spawn
            weaponAimActive = true;
            for (int i = 0; i < 3; i++)
            {
                timeToNextShot = shootAimTime;
                while (timeToNextShot > 0)
                {
                    timeToNextShot -= Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }
                StartCoroutine(ShootAttackRoutine());
            }
            weaponAimActive = false;

            yield return new WaitForSeconds(timeBetweenAttacks);
            //StartCoroutine(SpawnRatAttackRoutine());
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    IEnumerator ShootAttackRoutine()
    {
        Debug.Log("PANG");
        yield break;    
        
    }


    void AimLineVFX()
    {
        aimRayHit = Physics2D.Raycast(weaponTransform.position, weaponTransform.right, 200f, hitLayerMask);
        

        aimLineRenderer.SetPosition(0, weaponTransform.position);
        aimLineRenderer.SetPosition(1, aimRayHit.point);   



    }

    IEnumerator SpawnRatAttackRoutine()
    {
        yield break;
    }


    // /// Gun /// //

    //private void DetermineTargetGunDirection()
    //{
    //    Debug.Log(timeToNextShot);
    //    Vector3 targetPos = player.PredictFuturePosition(timeToNextShot);
    //    testMarker.position = targetPos;

    //    targetDirection = (targetPos - weaponTransform.position).normalized;
    //}

    private void DetermineTargetGunDirection()
    {
        // Get the direction between weaponTransform and targetPos
        Vector2 targetDirection = (target - weaponTransform.position).normalized;

        // Calculate the angle between the current forward direction and the target direction
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        // Calculate the difference in angle between current rotation and target angle
        float angleDifference = Mathf.DeltaAngle(weaponTransform.eulerAngles.z, targetAngle);

        // Calculate the desired rotation speed based on the angle difference
        float desiredRotationSpeed = Mathf.Clamp(angleDifference * rotationAcceleration, -maxRotationSpeed, maxRotationSpeed);

        // Apply rotation speed to the weapon
        float newRotationSpeed = Mathf.MoveTowards(weaponTransform.GetComponent<Rigidbody2D>().angularVelocity, desiredRotationSpeed, Time.deltaTime * rotationAcceleration);
        newRotationSpeed = Mathf.Clamp(newRotationSpeed, -maxAngularVelocity, maxAngularVelocity);
        weaponTransform.GetComponent<Rigidbody2D>().angularVelocity = newRotationSpeed;
    }

}
