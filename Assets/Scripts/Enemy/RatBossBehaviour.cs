using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RatBossBehaviour : Enemy
{
    [Serializable]
    private struct StartAndEndValue<T>
    {
        public T startVal;
        public T endVal;
    }

    [Header("General")] [SerializeField] float moveSpeed = 1f;
    [SerializeField] float changeDirectionInterval = 2f;
    [SerializeField] float followRange = 3f;
    [SerializeField] float timeBetweenAttacks = 4f;
    [SerializeField] LayerMask hitLayerMask;
    [SerializeField] GameObject healthPrefab;

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
    [SerializeField] AudioClip gunShotSFX;

    [Header("Gun Rotation")]
    [SerializeField] Transform weaponTransform; // The transform of the weapon
    [SerializeField] StartAndEndValue<float> maxRotationSpeed; // The rotation speed of the weapon
    [SerializeField] StartAndEndValue<float> rotationAcceleration; // The rotation acceleration towards the target direction
    [SerializeField] StartAndEndValue<float> maxAngularVelocity; // The angular velocity of the weapon
    [SerializeField] Vector2 facingLeftWeaponPoint;
    [SerializeField] Vector2 facingRightWeaponPoint;

    [Header("Attack - spawnRats")] 
    [SerializeField] GameObject ratsPrefab;
    [SerializeField] int amountOfRatsToSpawn;
    [SerializeField] AudioClip CaneBoomSFX;



    public BossRoom bossRoom;

    float currentMaxRotationSpeed;
    float currentRotationAcceleration;
    float currentMaxAngularVelocity;

    List<GameObject> ratMinions = new List<GameObject>();

    public enum RatState
    {
        paused,
        alive,
        dead
    }

    private bool isSummoning = false;
    private float timeToNextShot;
    private Vector3 toTarget;
    private RaycastHit2D aimRayHit;
    private Vector3 targetMovePosition;
    public RatState state = RatState.paused;
    private float elapsedTime;
    private Image healthBar;

    private LineRenderer aimLineRenderer;
    private BoxCollider2D roomCollider;
    private GameCamera cam;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private MusicManager musicManager;

    private void Awake()
    {
        currentMaxRotationSpeed = maxRotationSpeed.startVal;
        currentRotationAcceleration = rotationAcceleration.startVal;
        currentMaxAngularVelocity = maxAngularVelocity.startVal;
    }

    // Start is called before the first frame update
    protected override void EnemyStart()
    {
        aimLineRenderer = GetComponent<LineRenderer>();
        roomCollider = transform.parent.GetComponent<BoxCollider2D>();
        cam = FindObjectOfType<GameCamera>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        musicManager = FindObjectOfType<MusicManager>();

        aimLineRenderer.endColor = startAimColor;
        aimLineRenderer.startColor = startAimColor;
    }

    public void StartBoss()
    {
        musicManager.PlayRatBossTheme(false);
        SpawnHealthBar();
        state = RatState.alive;
        StartCoroutine(BossAttackLoopRoutine());

    }

    // Update is called once per frame
    void Update()
    {
        if (state != RatState.alive) return;

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
        for (int i = 0; i < 6; i++)
        {
            RaycastHit2D shootHit =
                Physics2D.Raycast(weaponTransform.position, weaponTransform.right, 200f, hitLayerMask);
            if (shootHit.point != Vector2.zero && shootHit.collider != null)
            {
                LineRenderer shootLine = Instantiate(shootLineVFX).GetComponent<LineRenderer>();
                shootLine.SetPosition(0, weaponTransform.position);
                shootLine.SetPosition(1, shootHit.point);

                if (shootHit.collider.CompareTag("Player"))
                {
                    player.TakeDamage(1);
                }
            }


            PlayGunSFX();

            yield return new WaitForSeconds(.15f);


        }

        yield return new WaitForSeconds(.3f);
        aimLineRenderer.enabled = true;
        yield break;
    }

    private void PlayGunSFX()
    {
        audioSource.PlayOneShot(gunShotSFX, .25f);
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

            if (aimRayHit.point != Vector2.zero)
            {
                aimLineRenderer.SetPosition(0, weaponTransform.position);
                aimLineRenderer.SetPosition(1, aimRayHit.point);
            }

        }
    }

    List<GameObject> _rats = new List<GameObject>();
    IEnumerator SpawnRatsRoutine()
    {
        animator.SetTrigger("summon");
        yield return new WaitForSeconds(0.9f); // (These are hardcoded, gl to whoever changed a sfx and needs to tweak this)
        PlayBoomSFX();
        yield return new WaitForSeconds(0.6f);
        cam.DoCameraShake();
        yield return new WaitForSeconds(.3f);

        _rats.Clear();
        for (int i = 0; i < amountOfRatsToSpawn; i++)
            _rats.Add(ratsPrefab);

        ratMinions.AddRange(GameHelper.InstanciateInCollider(roomCollider, _rats, LayerMask.GetMask("Wall")));
        yield break;
    }

    public void PlayBoomSFX()
    {
        audioSource.PlayOneShot(CaneBoomSFX, 1.5f);
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
        Instantiate(healthBarPrefab);
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Image>();
        healthBar.fillAmount = health / maxHealth;

    }

    private void UpdateHealthBar()
    {
        float healthRatio = health / maxHealth;
        healthBar.fillAmount = healthRatio;

        ScaleDifficulty(1-healthRatio);
    }

    public override void Die()
    {
        if (state != RatState.dead)
        {
            StopAllCoroutines();
            state = RatState.dead;
            weaponTransform.gameObject.SetActive(false);
            aimLineRenderer.enabled = false;
            animator.SetTrigger("die");
            musicManager.PlayExploringTheme();

            KillAllRatMinions();
            SpawnHP_Pickups();

            bossRoom.OnBossDead();
        }

    }

    private void KillAllRatMinions()
    {
        foreach (GameObject rat in ratMinions)
        {
            if (rat != null)
                rat.GetComponent<Enemy>().Die();

}
    }

    private void ScaleDifficulty(float t)
    {
        currentMaxRotationSpeed = GameHelper.MapValue(t, 0, 1, maxRotationSpeed.startVal, maxRotationSpeed.endVal);
        currentRotationAcceleration = GameHelper.MapValue(t, 0, 1, rotationAcceleration.startVal, rotationAcceleration.endVal);
        currentMaxAngularVelocity = GameHelper.MapValue(t, 0, 1, maxAngularVelocity.startVal, maxAngularVelocity.endVal);
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
            Mathf.Clamp(angleDifference * currentRotationAcceleration, -currentMaxRotationSpeed, currentMaxRotationSpeed);

        // Apply rotation speed to the weapon
        float newRotationSpeed = Mathf.MoveTowards(weaponTransform.GetComponent<Rigidbody2D>().angularVelocity,
            desiredRotationSpeed, Time.deltaTime * currentRotationAcceleration);
        newRotationSpeed = Mathf.Clamp(newRotationSpeed, -currentMaxAngularVelocity, currentMaxAngularVelocity);
        weaponTransform.GetComponent<Rigidbody2D>().angularVelocity = newRotationSpeed;
    }
    void SpawnHP_Pickups()
    {
        int amountOfHealth = UnityEngine.Random.Range(1, 5);
        for (int i = 0; i < amountOfHealth; i++)
        {
            Instantiate(healthPrefab, transform.position, Quaternion.identity);
        }
    }

    public void PlayShadowedCaneAnim()
    {
        animator.SetTrigger("shadowCane");
    }
}
