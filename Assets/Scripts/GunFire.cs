using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GunFire : MonoBehaviour
{
    enum WeaponType
    {
        ProjectileFire,
        RaycastFire,
        BurstFire
    }

    //Configurable parameters
    [Header("General Options")]
    [SerializeField] WeaponType weaponType;
    [SerializeField] float fireRate = 0.5f;
    [SerializeField] float reloadTime = 1f;
    [SerializeField, Tooltip("Set to 0 for infinite (Except for burst fire)")] int bulletsBeforeReload = 5;
    [SerializeField] GameObject hitEffect;
    [SerializeField] float destroyHitEffectAfter = 1.5f;

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

    Transform pivotPointTransform;
    Quaternion randomBulletSpread;
    RaycastHit2D bulletHit;

    private void Start()
    {
        reloadTimer = reloadTime;
        fireRateCooldownTimer = fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        pivotPointTransform = GetComponentInParent<Transform>();
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

        if (Input.GetKey(KeyCode.Mouse0))
        {
            randomBulletSpread = GetBulletSpread();
            Instantiate(projectile, transform.position, randomBulletSpread);
            fireRateCoolingDown = true;
            bulletsFired++;
        }
    }

    void BurstFire()
    {
        if(weaponType != WeaponType.BurstFire || fireRateCoolingDown || reloading) { return; }

        if(Input.GetKey(KeyCode.Mouse0))
        {
            for (int i = 0; i < bulletsBeforeReload || bulletsBeforeReload == 0; i++)
            {
                Debug.Log(i);
                randomBulletSpread = GetBulletSpread();
                Instantiate(projectile, transform.position, randomBulletSpread);
                bulletsFired++;
            }

        }
    }

    void RaycastFire()
    {
        if (weaponType != WeaponType.RaycastFire || fireRateCoolingDown || reloading) { return; }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            randomBulletSpread = GetBulletSpread();

            bulletHit = Physics2D.Raycast(transform.position, transform.right);
            if (bulletHit)
            {
                bulletsFired++;
                fireRateCoolingDown = true;

                var smoke = Instantiate(hitEffect, bulletHit.point, Quaternion.identity);
                Destroy(smoke, destroyHitEffectAfter);

                var line = Instantiate(bulletTrail, transform.position, Quaternion.identity);
                Destroy(line, destroyTrailAfter);
            }
            else
            {
                var smoke = Instantiate(hitEffect, bulletHit.point, Quaternion.identity);
                Destroy(smoke, destroyHitEffectAfter);
            }
        }
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
        reloadTimer -= Time.deltaTime;
        if(reloadTimer <= 0)
        {
            bulletsFired = 0;
            reloading = false;
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

    public Vector3 GetBulletHitPoint()
    {
        return bulletHit.point;
    }

    public GameObject GetHitEffect()
    {
        return hitEffect;
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
