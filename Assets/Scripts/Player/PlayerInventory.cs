using UnityEngine;


public class PlayerInventory : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] Transform holdableItemAnchor;
    [SerializeField] GameObject[] itemInventory;
    [SerializeField] GameObject itemPickupPrefab;



    // Hotbar
    int currentHotbarPos = 0;

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
            SpawnItemPickup(cHotbarItem);
            IItem oldItem = GameHelper.GetComponentInAllChildren<IItem>(cHotbarItem);
            if (oldItem != null)
            {
                Debug.Log("Found OldItem IItem");
                oldItem.DeActivate();
            }
            cHotbarItem.SetActive(false);
        }
        
        
        // Add new weapon to current hotbar pos
        
        if(weapon.transform.IsChildOf(holdableItemAnchor))
        {
            //Debug.Log("Found pickup Content in hierarchy, Rebasing");

            weapon.transform.SetParent(holdableItemAnchor);
            itemInventory[currentHotbarPos] = weapon;
        }
        else
        {
            //Debug.Log("Pickup Content not in hierarchy, Spawning new");
            itemInventory[currentHotbarPos] = Instantiate(weapon, holdableItemAnchor);
        }
        
        // initiate weapon
        cHotbarItem = itemInventory[currentHotbarPos];
        
        cHotbarItem.SetActive(true);

        IItem newItem = GameHelper.GetComponentInAllChildren<IItem>(cHotbarItem);
        if (newItem != null)
        {
            Debug.Log("Found NewItem IItem");
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

    void SpawnItemPickup(GameObject item)
    {
        GameObject pickupObject = Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);
        pickupObject.name = item.name;

        // make weapon child of pickup
        //content.transform.SetParent(pickup.transform);

        ItemPickupInteractable pickupScript = pickupObject.GetComponent<ItemPickupInteractable>();
        pickupScript.item = item;
    }
}
