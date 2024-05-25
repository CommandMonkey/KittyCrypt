using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private Transform holdableItemAnchor;
    public GameObject[] itemInventory;
    [SerializeField] private GameObject itemPickupPrefab;

    // Hotbar
    public int currentHotbarPos = 0;

    // References
    private UserInput userInput;

    private void Start()
    {
        InitializeUserInput();
        UpdateWeapon();
    }

    private void InitializeUserInput()
    {
        userInput = FindObjectOfType<UserInput>();
        if (userInput != null)
        {
            userInput.onScroll.AddListener(OnScroll);
        }
    }

    public void AddWeapon(GameObject weapon)
    {
        currentHotbarPos = GetEmptyHotbarPos();
        UpdateWeapon();

        GameObject currentHotbarItem = itemInventory[currentHotbarPos];
        HandleCurrentHotbarItem(currentHotbarItem);

        AddWeaponToHotbar(weapon);
        ResetHotbarItemPosition();
    }

    private int GetEmptyHotbarPos()
    {
        for (int i = 0; i < itemInventory.Length; i++)
        {
            if (itemInventory[i] == null || GameHelper.GetComponentInAllChildren<Item>(itemInventory[i].transform) == null)
                return i;
        }
        return currentHotbarPos;
    }

    private void HandleCurrentHotbarItem(GameObject currentHotbarItem)
    {
        if (currentHotbarItem != null)
        {
            Item oldItem = GameHelper.GetComponentInAllChildren<Item>(currentHotbarItem.transform);
            if (oldItem != null)
            {
                SpawnItemPickup(currentHotbarItem, oldItem.itemName);
                oldItem.DeActivate();
                currentHotbarItem.SetActive(false);
            }
        }
    }

    private void AddWeaponToHotbar(GameObject weapon)
    {
        if (weapon.transform.IsChildOf(holdableItemAnchor))
        {
            weapon.transform.SetParent(holdableItemAnchor);
            weapon.gameObject.SetActive(true);
            itemInventory[currentHotbarPos] = weapon;

            Item currentItem = GameHelper.GetComponentInAllChildren<Item>(weapon.transform);
            currentItem?.Activate();
        }
        else
        {
            itemInventory[currentHotbarPos] = Instantiate(weapon, holdableItemAnchor);
            itemInventory[currentHotbarPos].gameObject.SetActive(true);
        }
    }

    private void ResetHotbarItemPosition()
    {
        GameObject currentHotbarItem = itemInventory[currentHotbarPos];
        if (currentHotbarItem != null)
        {
            currentHotbarItem.transform.localPosition = Vector3.zero;
        }
    }

    private void OnScroll(float scrollValue)
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

    private void AddToHotbarPos(int value)
    {
        int newIndex = (currentHotbarPos + value) % itemInventory.Length;
        if (newIndex < 0)
        {
            newIndex += itemInventory.Length;
        }
        currentHotbarPos = newIndex;
        UpdateWeapon();
    }

    private void UpdateWeapon()
    {
        for (int i = 0; i < itemInventory.Length; i++)
        {
            GameObject weapon = itemInventory[i];
            if (weapon != null)
            {
                Item itemScript = GameHelper.GetComponentInAllChildren<Item>(weapon.transform);
                if (itemScript != null)
                {
                    if (i == currentHotbarPos)
                    {
                        itemScript.Activate();
                        weapon.SetActive(true);
                    }
                    else
                    {
                        itemScript.DeActivate();
                        weapon.SetActive(false);
                    }
                }
            }
        }
    }

    private void SpawnItemPickup(GameObject itemObj, string name)
    {
        GameObject pickupObject = Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);
        ItemPickupInteractable pickupScript = pickupObject.GetComponent<ItemPickupInteractable>();
        if (pickupScript != null)
        {
            pickupScript.item = itemObj;

            Item pickupItem = GameHelper.GetComponentInAllChildren<Item>(itemObj.transform);
            pickupItem?.DeActivate();
            if (pickupItem != null)
            {
                pickupItem.itemName = name;
            }
        }
    }
}
