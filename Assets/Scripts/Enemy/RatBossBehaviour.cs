using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RatBossBehaviour : Enemy
{
    [Header("General")] [SerializeField] float moveSpeed = 1f;
    [SerializeField] float changeDirectionInterval = 2f;
    [SerializeField] float followRange = 3f;
    [SerializeField] float timeBetweenAttacks = 4f;
    [SerializeField] LayerMask hitLayerMask;

    [Header("Health Display")]
    [SerializeField] GameObject healthBarPrefab;

    [Header("Attack - shoot")] [SerializeField]
    float shootAimTime = 3f;

    [SerializeField] GameObject aimLineVFX;
    [SerializeField] GameObject shootLineVFX;
    [SerializeField] Color startAimColor;
    [SerializeField] Color endAimColor;
    [SerializeField] float startAimWidth;
    [SerializeField] float endAimWidth;
    [SerializeField] Transform weaponTransform; // The transform of the weapon
    [SerializeField] float maxRotationSpeed = 5f; // The maximum rotation speed of the weapon
    [SerializeField] float rotationAcceleration = 10f; // The rotation acceleration towards the target direction
    [SerializeField] float maxAngularVelocity = 10f; // The maximum angular velocity of the weapon
    [SerializeField] Vector2 facingLeftWeaponPoint;
    [SerializeField] Vector2 facingRightWeaponPoint;

    [Header("Attack - spawnRats")] [SerializeField]
    List<GameObject> ratsToSummon;
    [SerializeField] AudioClip CaneBoomSFX;

    private enum RatState
    {
        paused,
        alive,
        dead
    }

    private bool isSummoning = false;
    private float timeToNextShot;
    private Vector3 toTarget;
    private RaycastHit2D aimRayHit;
    private RatState state = RatState.paused;
    private Vector3 targetMovePosition;
    private float elapsedTime;
    private Image healthBar;

    private LineRenderer aimLineRenderer;
    private BoxCollider2D roomCollider;
    private GameCamera cam;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    protected override void EnemyStart()
    {
        aimLineRenderer = GetComponent<LineRenderer>();
        roomCollider = transform.parent.GetComponent<BoxCollider2D>();
        cam = FindObjectOfType<GameCamera>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();


        aimLineRenderer.endColor = startAimColor;
        aimLineRenderer.startColor = startAimColor;

        SpawnHealthBar();
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

        ShootLineOfSightRay();
        toTarget = targetPosition - transform.position;
        

        if (isSummoning)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
            Movement();
            Flip();

        }

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

                    // Calculate the interpolation factor (t) based on time
                    float t = 1f - (timeToNextShot / shootAimTime);

                    // Interpolate the color gradually
                    aimLineRenderer.endColor = Color.Lerp(startAimColor, endAimColor, t);
                    aimLineRenderer.startColor = Color.Lerp(startAimColor, endAimColor, t);

                    // Interpolate the width gradually
                    aimLineRenderer.startWidth = Mathf.Lerp(startAimWidth, endAimWidth, t);
                    aimLineRenderer.endWidth = Mathf.Lerp(startAimWidth, endAimWidth, t);

                    yield return new WaitForFixedUpdate();
                }

                aimLineRenderer.enabled = false;
                yield return StartCoroutine(ShootAttackRoutine());
            }

            aimLineRenderer.enabled = false;

            isSummoning = true;
            yield return StartCoroutine(SpawnRatsRoutine());
            yield return new WaitForSeconds(timeBetweenAttacks);
            isSummoning = false;
        }
    }

    IEnumerator ShootAttackRoutine()
    {
        for (int i = 0; i < 5; i++)
        {
            RaycastHit2D shootHit =
                Physics2D.Raycast(weaponTransform.position, weaponTransform.right, 200f, hitLayerMask);

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


    void Movement()
    {
        if (toTarget.magnitude > followRange)
            // Move towards the target position
            transform.position += (Vector3)toTarget.normalized * moveSpeed * Time.deltaTime;

        // Change target position after a certain interval
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= changeDirectionInterval)
        {
            SetRandomTargetPosition();
            elapsedTime = 0f;
        }

    }

    void SetRandomTargetPosition()
    {
        // Generate random target position within a range from the current position
        targetMovePosition = player.transform.position +
                             new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
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
        yield return new WaitForSeconds(1.5f); // Wait for K�pp to hit the ground
        cam.DoCameraShake();
        PlayBoomSFX();
        yield return new WaitForSeconds(.3f);
        GameHelper.InstanciateInCollider(roomCollider, ratsToSummon, LayerMask.GetMask("Wall"));
        yield break;
    }

    void PlayBoomSFX()
    {
        audioSource.PlayOneShot(CaneBoomSFX);
    }

    void Flip()
    {
        if (toTarget.x < 0f)
        {
            spriteRenderer.flipX = false;
            weaponTransform.localPosition = facingLeftWeaponPoint;
        }
        else if (toTarget.x > 0f)
        {
            spriteRenderer.flipX = true;
            weaponTransform.localPosition = facingRightWeaponPoint;
        }
    }

    protected override void OnDamageTaken()
    {
        UpdateHealthBar();
    }

    private void SpawnHealthBar()
    {
        healthBar = GameHelper.GetComponentInAllChildren<Image>(Instantiate(healthBarPrefab));
        healthBar.fillAmount = health / maxHealth;

    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = health / maxHealth;
    }

    protected override void Die()
    {
        
    }


    // /// Gun /// //


    private void DetermineTargetGunDirection()
    {
        // Get the direction between weaponTransform and targetPos
        Vector2 targetDirection = (targetPosition - weaponTransform.position).normalized;

        // Calculate the angle between the current forward direction and the target direction
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        // Calculate the difference in angle between current rotation and target angle
        float angleDifference = Mathf.DeltaAngle(weaponTransform.eulerAngles.z, targetAngle);

        // Calculate the desired rotation speed based on the angle difference
        float desiredRotationSpeed =
            Mathf.Clamp(angleDifference * rotationAcceleration, -maxRotationSpeed, maxRotationSpeed);

        // Apply rotation speed to the weapon
        float newRotationSpeed = Mathf.MoveTowards(weaponTransform.GetComponent<Rigidbody2D>().angularVelocity,
            desiredRotationSpeed, Time.deltaTime * rotationAcceleration);
        newRotationSpeed = Mathf.Clamp(newRotationSpeed, -maxAngularVelocity, maxAngularVelocity);
        weaponTransform.GetComponent<Rigidbody2D>().angularVelocity = newRotationSpeed;
    }

}
