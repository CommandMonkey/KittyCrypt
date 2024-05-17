using UnityEngine;


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

        // input events
        userInput.onScroll.AddListener(OnScroll);

        UpdateWeapon();
    }

    public void AddWeapon(GameObject weapon)
    {
        GameObject cHotbarItem = itemInventory[currentHotbarPos];
        
        // drop item if holding one
        if (cHotbarItem != null)
        {
            Item oldItem = GameHelper.GetComponentInAllChildren<Item>(cHotbarItem);
            SpawnItemPickup(cHotbarItem, oldItem.name);
            if (oldItem != null)
            {
                oldItem.DeActivate();
            }
            cHotbarItem.SetActive(false);
        }
        
        
        // Add new weapon to current hotbar pos
        
        if(weapon.transform.IsChildOf(holdableItemAnchor))
        {
            weapon.transform.SetParent(holdableItemAnchor);
            itemInventory[currentHotbarPos] = weapon;
        }
        else
        {
            itemInventory[currentHotbarPos] = Instantiate(weapon, holdableItemAnchor);
        }
        
        // initiate weapon
        cHotbarItem = itemInventory[currentHotbarPos];
        
        cHotbarItem.SetActive(true);

        Item newItem = GameHelper.GetComponentInAllChildren<Item>(cHotbarItem);
        if (newItem != null)
        {
            newItem.Activate();
        }
        
        // reset local pos (cus had some problem or smthn)
        itemInventory[currentHotbarPos].transform.localPosition = Vector3.zero;
    }

    void OnScroll(float scrollValue)
    {
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
            newIndex += itemInventory.Length;
        }

        currentHotbarPos = newIndex % itemInventory.Length;

        UpdateWeapon();
    }


    void UpdateWeapon()
    {
        //if (!weaponInventory[currentHotbarPos].activeSelf) return;

        foreach (GameObject weapon in itemInventory)
        {
            if (weapon != null && weapon == itemInventory[currentHotbarPos]) 
            {
                weapon.SetActive(true);
            }
            else if (weapon != null)
            {
                weapon.SetActive(false);
            }
        }

    }


    // Pickup Helper

    void SpawnItemPickup(GameObject itemObj, string name)
    {
        GameObject pickupObject = Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);


        // make weapon child of pickup
        //content.transform.SetParent(pickup.transform);

        ItemPickupInteractable pickupScript = pickupObject.GetComponent<ItemPickupInteractable>();
        pickupScript.item = itemObj;

        // Set name
        GameObject pickupItem = pickupObject.GetComponent<ItemPickupInteractable>().item;
        if (pickupItem != null)
        {
            Item itemitem = pickupItem.GetComponent<Item>();
            if (itemitem != null) itemitem.itemName = name;
        }
    }
}
