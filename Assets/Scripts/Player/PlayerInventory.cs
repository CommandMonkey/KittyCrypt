using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;

public class PlayerInventory : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] Transform weaponAnchor;
    [SerializeField] GameObject[] weaponInventory;

    [Header("Pickup")]
    [SerializeField] TMP_Text pickupPromptText;
    [SerializeField] GameObject pickupPrefab;


    // Hotbar
    int currentHotbarPos = 0;

    // Pickup
    bool anyPickupsInRange = false;
    List<Pickup> pickupsInRange = new List<Pickup>();
    Pickup closestPickup;

    // References
    LevelManager gameManager;


    private void Start()
    {
        gameManager = FindObjectOfType<LevelManager>();

        UpdateWeapon();
    }


    private void FixedUpdate()
    {
        if (anyPickupsInRange)
        {
            closestPickup = GetClosestPickup();
            UpdatePickupPrompt();
        }
    }

    void AddWeapon(GameObject weapon)
    {
        GameObject cHotbarSpot = weaponInventory[currentHotbarPos];
        if (cHotbarSpot != null)
        {
            SpawnPickup(cHotbarSpot, PickupType.Weapon);
            cHotbarSpot.SetActive(false);
        }
        

        if(weapon.transform.IsChildOf(weaponAnchor))
        {
            //Debug.Log("Found pickup Content in hierarchy, Rebasing");

            weapon.transform.SetParent(weaponAnchor);
            weapon.SetActive(true);
            weaponInventory[currentHotbarPos] = weapon;
        }
        else
        {
            //Debug.Log("Pickup Content not in hierarchy, Spawning new");
            weaponInventory[currentHotbarPos] = Instantiate(weapon, weaponAnchor);
        }

        // reset local pos (cus had some problem or smthn)
        weaponInventory[currentHotbarPos].transform.localPosition = Vector3.zero;
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
        int newIndex = currentHotbarPos + value;

        // Ensure the index stays within bounds
        while (newIndex < 0)
        {
            newIndex += weaponInventory.Length;
        }

        currentHotbarPos = newIndex % weaponInventory.Length;

        UpdateWeapon();
    }


    void UpdateWeapon()
    {
        //if (!weaponInventory[currentHotbarPos].activeSelf) return;

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

    // PICKUP GROUND ITEMS
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Pickup")) return;

        pickupsInRange.Add(collision.gameObject.GetComponent<Pickup>());
        anyPickupsInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Pickup pickup = collision.gameObject.GetComponent<Pickup>();
        if (pickupsInRange.Contains(pickup))
        {
            pickupsInRange.Remove(pickup);
            if (pickupsInRange.Count == 0)
            {
                anyPickupsInRange = false;
                pickupPromptText.text = "";
            }
                
        }
    }

    void OnInteract()
    {
        if (anyPickupsInRange && closestPickup.type == PickupType.Weapon)
        {
            AddWeapon(closestPickup.content);
            Destroy(closestPickup.gameObject);

            UpdateWeapon();
        }
    }

    private void UpdatePickupPrompt()
    {
        Vector2 textScreenPosition = gameManager.mainCamera.WorldToScreenPoint(closestPickup.transform.position);

        pickupPromptText.transform.position = textScreenPosition;
        pickupPromptText.text = "E to pick up: " + closestPickup.name;
    }

    // Pickup Helper

    Pickup GetClosestPickup()
    {
        Pickup closest = pickupsInRange[0];
        Vector2 closestToMouse;
        foreach (Pickup pickup in pickupsInRange)
        {
            closestToMouse = closest.transform.position - transform.position;
            
            if (closestToMouse.magnitude > (pickup.transform.position - transform.position).magnitude)
            {
                closest = pickup;
            }
        }

        return closest;
    }

    void SpawnPickup(GameObject content, PickupType type)
    {
        GameObject pickup = Instantiate(pickupPrefab, transform.position, Quaternion.identity);
        pickup.name = content.name;

        // make weapon child of pickup
        //content.transform.SetParent(pickup.transform);

        Pickup pickupScript = pickup.GetComponent<Pickup>();
        pickupScript.content = content;
        pickupScript.type = type;
    }

}
