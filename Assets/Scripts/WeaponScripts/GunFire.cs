using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GunFire : MonoBehaviour
{
    enum WeaponType
    {
        ProjectileFire,
        BurstFire,
        RaycastFire
    }

    //Configurable parameters damage
    [Header("General Options")]
    [SerializeField] WeaponType weaponType;
    [SerializeField] AudioClip fireAudio;
    [SerializeField] AudioClip reloadAudio;
    [SerializeField] float fireRate = 0.5f;
    [SerializeField] float reloadTime = 1f;
    [SerializeField] float damagePerBullet = 1f;
    [SerializeField, Tooltip("Set to 0 for infinite (Except for burst fire)")] int bulletsBeforeReload = 5;
    [SerializeField] GameObject hitEffect;
    [SerializeField] float destroyHitEffectAfter = 1.5f;
    [SerializeField] float knockback = 2f;
    [SerializeField] LayerMask ignoreLayerMask;

    [Header("Projectile Options")]
    [SerializeField] GameObject projectile;
    [SerializeField, Range(0f, 180f), Tooltip("Range goes in both directions")] float bulletSpreadRange = 1f;
    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] float projectileVanishAfter = 3f;

    [Header("Raycast Options")]
    [SerializeField] GameObject bulletTrail;
    [SerializeField] float destroyTrailAfter = .1f;


    //Cached references

    //Private variables
    int bulletsFired;
    float reloadTimer;
    float fireRateCooldownTimer;
    bool fireRateCoolingDown = false;
    bool reloading = false;
    bool reloadAudioPlayed = false;

    PlayerInput playerInput;
    PlayerMovement playerMovement;
    Animator virtualCameraAnimator;
    Quaternion randomBulletSpread;
    RaycastHit2D bulletHit;
    AudioSource gunSource;

    private void Start()
    {
        reloadTimer = reloadTime;
        fireRateCooldownTimer = fireRate;
        playerInput = GetComponent<PlayerInput>();
        virtualCameraAnimator = FindObjectOfType<CinemachineVirtualCamera>().GetComponent<Animator>();
        gunSource = FindObjectOfType<LevelManager>().gameObject.GetComponent<AudioSource>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        ProjectileFire();
        BurstFire();
        RaycastFire();
        Reload();
        CheckBulletsFired();
        FireRateCooldown();
    }

    void ProjectileFire()
    {
        if(weaponType != WeaponType.ProjectileFire || fireRateCoolingDown || reloading) { return; }

        if (playerInput.actions["Fire"].IsPressed()) 
        {
            randomBulletSpread = GetBulletSpread();
            Instantiate(projectile, transform.position, randomBulletSpread);
            GunFeedbackEffects();
            fireRateCoolingDown = true;
            bulletsFired++;
        }
    }

    void BurstFire()
    {
        if(weaponType != WeaponType.BurstFire || fireRateCoolingDown || reloading) { return; }

        if(playerInput.actions["Fire"].IsPressed())
        {
            for (int i = 0; i < bulletsBeforeReload || bulletsBeforeReload == 0; i++)
            {
                randomBulletSpread = GetBulletSpread();
                Instantiate(projectile, transform.position, randomBulletSpread);
                bulletsFired++;
            }
            GunFeedbackEffects();
        }
    }

    void RaycastFire()
    {
        if (weaponType != WeaponType.RaycastFire || fireRateCoolingDown || reloading) { return; }

        if (playerInput.actions["Fire"].IsPressed())
        {
            randomBulletSpread = GetBulletSpread();

            bulletHit = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, ~ignoreLayerMask);
            if (bulletHit)
            {
                bulletsFired++;
                GunFeedbackEffects();
                fireRateCoolingDown = true;

                var smoke = Instantiate(hitEffect, bulletHit.point, Quaternion.identity);
                Destroy(smoke, destroyHitEffectAfter);

                var line = Instantiate(bulletTrail, transform.position, Quaternion.identity);
                Destroy(line, destroyTrailAfter);

                if (bulletHit.collider.gameObject.transform.tag == "Enemy")
                {
                    RushingEnemyBehavior enemyScript = bulletHit.collider.gameObject.GetComponent<RushingEnemyBehavior>();

                    enemyScript.TakeDamage(damagePerBullet);
                }
            }
        }
    }

    void GunFeedbackEffects()
    {
        gunSource.PlayOneShot(fireAudio);
        virtualCameraAnimator.SetTrigger("CameraShake");
        playerMovement.exteriorVelocity += -(Vector2)transform.right * knockback;
    }

    void CheckBulletsFired()
    {
        if(bulletsBeforeReload == 0) { return; }

        if (bulletsFired >= bulletsBeforeReload)
        {
            reloading = true;
        }
    }

    void FireRateCooldown()
    {
        if(!fireRateCoolingDown) { return; }

        fireRateCooldownTimer -= Time.deltaTime;
        if(fireRateCooldownTimer <= 0 )
        {
            fireRateCoolingDown = false;
            fireRateCooldownTimer = fireRate;
        }
    }

    void Reload()
    {
        if (!reloading) { return; }
        if (!reloadAudioPlayed)
        {
            gunSource.PlayOneShot(reloadAudio);
            reloadAudioPlayed = true;
        }

        reloadTimer -= Time.deltaTime;
        if(reloadTimer <= 0)
        {
            bulletsFired = 0;
            reloading = false;
            reloadAudioPlayed = false;
            reloadTimer = reloadTime;
        }
    }

    public float GetProjectileSpeed()
    {
        return projectileSpeed;
    }

    public float GetVanishAfter()
    {
        return projectileVanishAfter;
    }

    public float GetDamagePerBullet()
    {
        return damagePerBullet;
    }

    public Vector3 GetBulletHitPoint()
    {
        return bulletHit.point;
    }

    public GameObject GetHitEffect()
    {
        return hitEffect;
    }

    public LayerMask GetIgnoredLayers()
    {
        return ignoreLayerMask;
    }

    public float GetDestroyHitEffectAfter()
    {
        return destroyHitEffectAfter;
    }

    private Quaternion GetBulletSpread()
    {
        Vector3 angles = transform.rotation.eulerAngles;
        Vector3 newAngles = new Vector3(
            angles.x,
            angles.y,
            angles.z + Random.Range(-bulletSpreadRange, bulletSpreadRange));
        return Quaternion.Euler(newAngles);
    }
}
