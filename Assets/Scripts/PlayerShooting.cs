using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    enum WeaponType
    {
        ProjectileFire,
        RaycastFire
    }

    [SerializeField] WeaponType weaponType;

    //Configurable parameters
    [SerializeField] float reloadSpeed = 0.5f;
    
    //Cached references
    [SerializeField] GameObject bullet;

    //Private variables

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 lookRotation = transform.position - mousePos;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, lookRotation);
        
        ProjectileFire(lookRotation);
        RaycastFire(lookRotation);
    }

    void ProjectileFire(Vector3 lookRotation)
    {
        if(weaponType != WeaponType.ProjectileFire) { return; }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Debug.DrawRay(transform.position, -lookRotation);
            Instantiate(bullet, transform.position, transform.rotation);
        }
    }

    private void RaycastFire(Vector3 lookRotation)
    {
        if (weaponType != WeaponType.RaycastFire) { return; }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.DrawRay(transform.position, -lookRotation, Color.yellow);
            Debug.Log(-lookRotation);
        }
    }
}
