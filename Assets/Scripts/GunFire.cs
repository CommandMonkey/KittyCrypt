using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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

    [Header("Projectile Fire Options")]
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed;
    [SerializeField] float projectileVanishAfter;

    [Header("Raycast Fire Options")]
    [SerializeField] float testVariable;


    //Cached references

    //Private variables
    float reloadTimer;
    bool reloading = false;

    private void Start()
    {
        reloadTimer = fireRate;
    }

    // Update is called once per frame
    void Update()
    {   
        

        ProjectileFire();
        RaycastFire();
        Reload();
    }

    void ProjectileFire()
    {
        if(weaponType != WeaponType.ProjectileFire) { return; }

        if (Input.GetKey(KeyCode.Mouse0) && !reloading)
        {
            Instantiate(projectile, transform.position, transform.rotation);

            reloading = true;
        }
    }

    void RaycastFire()
    {
        if (weaponType != WeaponType.RaycastFire) { return; }

        if (Input.GetKey(KeyCode.Mouse0) && !reloading)
        {
            Debug.DrawRay(transform.position, -transform.up * 10, Color.yellow);

            reloading = true;
        }
    }

    void Reload()
    {
        if (!reloading) { return; }
        reloadTimer -= Time.deltaTime;
        if(reloadTimer < 0)
        {
            reloading = false;
            reloadTimer = fireRate;
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
