using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] Transform holdableItemAnchor;
    public GameObject[] itemInventory;
    [SerializeField] GameObject itemPickupPrefab;

    // Hotbar
    public int currentHotbarPos = 0;

    // References
    UserInput userInput;

    private void Start()
    {
        userInput = FindObjectOfType<UserInput>();

        // Input events
        if (userInput != null)
        {
            userInput.onScroll.AddListener(OnScroll);
        }

        UpdateWeapon();
    }

    public void AddWeapon(GameObject weapon)
    {
        currentHotbarPos = GetEmptyHotBarPos();
        UpdateWeapon();
        GameObject currentHotbarItem = itemInventory[currentHotbarPos];

        // Drop item if holding one
        if (currentHotbarItem != null)
        {
            Item oldItem = GameHelper.GetComponentInAllChildren<Item>(currentHotbarItem.transform);
            if (oldItem != null)
            {
                SpawnItemPickup(currentHotbarItem, oldItem.name);
                oldItem.DeActivate();
                currentHotbarItem.SetActive(false);
            }
        }

        // Add new weapon to current hotbar position
        if (weapon.transform.IsChildOf(holdableItemAnchor))
        {
            weapon.transform.SetParent(holdableItemAnchor);
            itemInventory[currentHotbarPos] = weapon;
        }
        else
        {
            itemInventory[currentHotbarPos] = Instantiate(weapon, holdableItemAnchor);
        }

        // Initiate weapon
        currentHotbarItem = itemInventory[currentHotbarPos];

        if (currentHotbarItem != null)
        {
            currentHotbarItem.SetActive(true);

            //Item newItem = GameHelper.GetComponentInAllChildren<Item>(currentHotbarItem.transform);
            //if (newItem != null)
            //{
            //    newItem.Activate();
            //}

            // Reset local position
            currentHotbarItem.transform.localPosition = Vector3.zero;
        }
    }

    private int GetEmptyHotBarPos()
    {
        for (int i = 0; i < itemInventory.Length; i++)
        {
            if (itemInventory[i] == null)
                return i;

            if (GameHelper.GetComponentInAllChildren<Item>(itemInventory[i].transform) == null)
                return i;
        }
        return currentHotbarPos;
    }



    void OnScroll(float scrollValue)
    {
        if (scrollValue > 0)
        {
            AddToHotbarPos(1);
        }
        else if (scrollValue < 0)
        {
            AddToHotbarPos(-1);
        }
    }

    void AddToHotbarPos(int value)
    {
        int newIndex = (currentHotbarPos + value) % itemInventory.Length;

        // Ensure the index stays within bounds
        if (newIndex < 0)
        {
            newIndex += itemInventory.Length;
        }

        currentHotbarPos = newIndex;

        UpdateWeapon();
    }

    void UpdateWeapon()
    {
        for (int i = 0; i < itemInventory.Length; i++)
        {
            GameObject weapon = itemInventory[i];
            Debug.Log("hotbarpos: " + i);
            if (weapon != null)
            {
                Item itemScript = GameHelper.GetComponentInAllChildren<Item>(weapon.transform);
                if (itemScript != null)
                {
                    if (i == currentHotbarPos)
                    {
                        itemScript.Activate();
                        weapon.SetActive(true);

                        Debug.Log("hotbarpos Holding: " + i + ", item: " + itemScript.itemName);
                    }
                    else
                    {
                        itemScript.DeActivate();
                        weapon.SetActive(false);
                    }
                }
            }
            else if (i == currentHotbarPos)
            {

            }
        }
    }

    // Pickup Helper
    void SpawnItemPickup(GameObject itemObj, string name)
    {
        GameObject pickupObject = Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);

        ItemPickupInteractable pickupScript = pickupObject.GetComponent<ItemPickupInteractable>();
        if (pickupScript != null)
        {
            pickupScript.item = itemObj;

            // Set name
            Item pickupItem = itemObj.GetComponent<Item>();
            if (pickupItem != null)
            {
                pickupItem.itemName = name;
            }
        }
    }
}
