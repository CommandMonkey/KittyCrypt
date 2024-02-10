using Unity.VisualScripting;
using UnityEngine;

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
    [SerializeField] Transform pivotPoint;
    [SerializeField] float fireRate = 0.5f;
    [SerializeField] float reloadTime = 1f;
    [SerializeField, Tooltip("Set to 0 for infinite")] int bulletsBeforeReload = 5;
    [SerializeField, Range(0f, 180f)] float bulletSpreadRange = 1f;

    [Header("Projectile Options")]
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] float projectileVanishAfter = 3f;

    [Header("Raycast Options")]
    [SerializeField] float fireDistance = 10f;
    [SerializeField] GameObject hitEffect;
    [SerializeField] float destroySmokeAfter = 1.5f;
    [SerializeField] GameObject bulletTrail;
    [SerializeField] float destroyTrailAfter = .1f;


    //Cached references

    //Private variables
    int bulletsFired;
    float reloadTimer;
    float fireRateCooldownTimer;
    bool fireRateCoolingDown = false;
    bool reloading = false;
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
        ProjectileFire();
        BurstFire();
        RaycastFire();
        Reload();
        CheckBulletsFired();
        FireRateCooldown();
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
        if(weaponType != WeaponType.BurstFire || reloading) { return; }

        if(Input.GetKey(KeyCode.Mouse0))
        {
            for (int i = 0; i < bulletsBeforeReload; i++)
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
            bulletHit = Physics2D.Raycast(transform.position, -transform.up);
            if (bulletHit)
            {
                var smoke = Instantiate(hitEffect, bulletHit.point, Quaternion.identity);
                Destroy(smoke, destroySmokeAfter);

                var line = Instantiate(bulletTrail, transform.position, Quaternion.identity);
                Destroy(line, destroyTrailAfter);
            }
            else
            {
                Debug.Log("No hit :(");
            }
            bulletsFired++;
            fireRateCoolingDown = true;
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
}
