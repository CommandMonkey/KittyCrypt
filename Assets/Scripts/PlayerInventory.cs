using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        for (int i = 0; i < weaponInventory.Length; i++)
        {
            if (weaponInventory[i] == null)
            {
                weaponInventory[i] = weapon;
            }
        }

    }

    void OnScroll(InputValue value)
    {
        Debug.Log(value.Get());
    }


}
