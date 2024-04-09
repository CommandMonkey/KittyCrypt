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
        GameObject cHotbarSpot = itemInventory[currentHotbarPos];
        if (cHotbarSpot != null)
        {
            SpawnItemPickup(cHotbarSpot);
            cHotbarSpot.SetActive(false);
        }
        

        if(weapon.transform.IsChildOf(holdableItemAnchor))
        {
            //Debug.Log("Found pickup Content in hierarchy, Rebasing");

            weapon.transform.SetParent(holdableItemAnchor);
            weapon.SetActive(true);
            itemInventory[currentHotbarPos] = weapon;
        }
        else
        {
            //Debug.Log("Pickup Content not in hierarchy, Spawning new");
            itemInventory[currentHotbarPos] = Instantiate(weapon, holdableItemAnchor);
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
        GameObject itemPickup = Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);
        itemPickup.name = item.name;

        // make weapon child of pickup
        //content.transform.SetParent(pickup.transform);

        ItemPickupInteractable pickupScript = itemPickup.GetComponent<ItemPickupInteractable>();
        pickupScript.item = item;
    }

}
