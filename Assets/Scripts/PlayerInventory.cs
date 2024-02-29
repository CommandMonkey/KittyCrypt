using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] Transform weaponAnchor;
    [SerializeField] GameObject[] weaponInventory;

    int currentHotbarPos = 0;

    private void Awake()
    {
        AddWeapon(new GameObject());
    }

    void AddWeapon(GameObject weapon)
    {
        for (int i = 0; i < weaponInventory.Length; i++)
        {
            if (weaponInventory[i] == null)
            {
                weaponInventory[i] = weapon;
                return;
            }
        }
    }

    void OnScroll(InputValue value)
    {
        float scrollValue = value.Get<float>();
        if (scrollValue > 0)
        {
            AddToHotbarPos(1);
        }
        else if(scrollValue < 0)
        {
            AddToHotbarPos(-1);
        }
    }

    void AddToHotbarPos(int value)
    {
        currentHotbarPos = (currentHotbarPos + value) % weaponInventory.Length;
        UpdateWeapon();
    }


    void UpdateWeapon()
    {
        if (weaponInventory[currentHotbarPos].activeSelf) return;

        foreach (GameObject weapon in weaponInventory)
        {
            if (weapon != null && weapon == weaponInventory[currentHotbarPos]) 
            {
                weapon.SetActive(true);
            }
            else if (weapon != null)
            {
                weapon.SetActive(false);
            }
        }

    }


}
