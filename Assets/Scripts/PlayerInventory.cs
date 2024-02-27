using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] Transform weaponAnchor;
    [SerializeField] int weaponInventorySize = 2;

    GameObject[] weaponInventory;

    private void Awake()
    {
        weaponInventory = new GameObject[weaponInventorySize];
        AddWeapon(new GameObject());
    }

    void AddWeapon(GameObject weapon)
    {
        //for (GameObject weaponItem in weaponInventory) 
        //{
        //    if (weaponItem == null)
        //    {
                
        //    }
        //}
        
    }


}
