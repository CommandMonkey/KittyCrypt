using Unity.VisualScripting;
using UnityEngine;

public class GunFire : MonoBehaviour
{
    enum WeaponType
    {
        ProjectileFire,
        RaycastFire
    }

    //Configurable parameters
    [Header("General Options")]
    [SerializeField] WeaponType weaponType;
    [SerializeField] Transform pivotPoint;
    [SerializeField] float fireRate = 0.5f;
    [SerializeField] float reloadTime = 1f;
    [SerializeField, Tooltip("Set to 0 for infinite")] int bulletsBeforeReload = 5;
    [SerializeField, Range(0f, 180f)] float bulletSpreadRange = 1f;

    [Header("Projectile Fire Options")]
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] float projectileVanishAfter = 3f;

    [Header("Raycast Fire Options")]
    [SerializeField] float fireDistance = 10f;


    //Cached references

    //Private variables
    int bulletsFired;
    float reloadTimer;
    float fireRateCooldownTimer;
    bool fireRateCoolingDown = false;
    bool reloading = false;
    Quaternion randomBulletSpread;

    private void Start()
    {
        reloadTimer = reloadTime;
        fireRateCooldownTimer = fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        ProjectileFire();
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
        
        randomBulletSpread = GetBulletSpread();

        if (Input.GetKey(KeyCode.Mouse0))
        {
            fireRateCoolingDown = true;
            Instantiate(projectile, transform.position, randomBulletSpread);
            bulletsFired++;
        }
    }

    void RaycastFire()
    {
        if (weaponType != WeaponType.RaycastFire || fireRateCoolingDown || reloading) { return; }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            //TODO: Make raycast firing work
            RaycastHit2D bulletHit = Physics2D.Raycast(transform.position, -transform.up);
            if (!bulletHit)
            {
                Debug.Log("Hit");
            }else
            {
                Debug.Log("No hit :(");
            }

            Debug.DrawRay(transform.position, -transform.up * fireDistance);
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
}
