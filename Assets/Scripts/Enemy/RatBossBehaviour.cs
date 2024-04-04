using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatBossBehaviour : MonoBehaviour, IEnemy
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
    [SerializeField] float maxRotationSpeed = 5f; // The maximum rotation speed of the weapon
    [SerializeField] float rotationAcceleration = 10f; // The rotation acceleration towards the target direction
    [SerializeField] float maxAngularVelocity = 10f; // The maximum angular velocity of the weapon
    [Header("Attack - spawnRats")]
    [SerializeField] List<GameObject> ratsToSummon;
    [SerializeField] AudioClip CaneBoomSFX;

    enum RatState
    {
        paused,
        alive,
        dead
    }
    bool weaponAimActive = false;

    float timeToNextShot;
    Vector3 target;
    RaycastHit2D aimRayHit;

    RatState state = RatState.paused;

    Player player;
    LineRenderer aimLineRenderer;
    BoxCollider2D roomCollider;
    Animator animator;
    GameCamera cam;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        aimLineRenderer = GetComponent<LineRenderer>();
        roomCollider = transform.parent.GetComponent<BoxCollider2D>();
        animator = GetComponentInChildren<Animator>();
        cam = FindObjectOfType<GameCamera>();
        audioSource = GetComponent<AudioSource>();

        StartBoss();
    }

    public void StartBoss()
    {
        state = RatState.alive;
        StartCoroutine(BossAttackLoopRoutine());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (state == RatState.paused) return;

        target = player.transform.position;
        DetermineTargetGunDirection();

        AimLineVFX();


    }



    IEnumerator BossAttackLoopRoutine()
    {
        while (state != RatState.dead)
        {
            // 3 shoot attacks then one rat spawn
            aimLineRenderer.enabled = true;
            for (int i = 0; i < 3; i++)
            {
                timeToNextShot = shootAimTime;
                while (timeToNextShot > 0)
                {
                    timeToNextShot -= Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }
                aimLineRenderer.enabled = false;
                yield return StartCoroutine(ShootAttackRoutine());
            }
            aimLineRenderer.enabled = false;


            yield return StartCoroutine(SpawnRatsRoutine());
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    IEnumerator ShootAttackRoutine()
    {
        
        for (int i = 0; i < 5; i++)
        {
            RaycastHit2D shootHit = Physics2D.Raycast(weaponTransform.position, weaponTransform.right, 200f, hitLayerMask);

            LineRenderer shootLine = Instantiate(shootLineVFX).GetComponent<LineRenderer>();
            shootLine.SetPosition(0, weaponTransform.position);
            shootLine.SetPosition(1, shootHit.point);

            if (shootHit.collider.CompareTag("Player"))
            {
                player.TakeDamage(1);
            }
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(.3f);
        aimLineRenderer.enabled = true;
        yield break;    
        
    }


    void AimLineVFX()
    {
        if (aimLineRenderer.enabled)
        {
            aimRayHit = Physics2D.Raycast(weaponTransform.position, weaponTransform.right, 200f, hitLayerMask);


            aimLineRenderer.SetPosition(0, weaponTransform.position);
            aimLineRenderer.SetPosition(1, aimRayHit.point);
        }
    }

    IEnumerator SpawnRatsRoutine()
    {
        animator.SetTrigger("summon");
        yield return new WaitForSeconds(1); // Wait for Käpp to hit the ground
        cam.DoCameraShake();
        PlayBoomSFX();
        yield return new WaitForSeconds(.3f); 
        EncounterRoom.SpawnEnemies(roomCollider, ratsToSummon, LayerMask.GetMask("Wall"));
        yield break;
    }

    void PlayBoomSFX()
    {
        audioSource.PlayOneShot(CaneBoomSFX);
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

    public void TakeDamage(float damage)
    {
        throw new NotImplementedException();
    }
}
