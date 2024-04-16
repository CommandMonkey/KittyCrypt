using UnityEngine;

public class ItemPickupInteractable : MonoBehaviour, IInteractable
{

    public GameObject item;
    [SerializeField] bool useSpawnRoomPool;
    public string interactPrompt { get; set; }
    public bool canInteract { get; set; } = true;

    [SerializeField] GameObject popVFX;
    
    GameSession gameSession;

    private void Start()
    {
        gameSession = GameSession.Instance;

        if (item == null)
        {
            item = useSpawnRoomPool ? gameSession.levelSettings.GetRandomSpawnRoomItem() : 
                                        gameSession.levelSettings.GetRandomItem();
        }

        interactPrompt = GameHelper.GetComponentInAllChildren<Item>(item).itemName;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null )
        {
            sr.sprite = GetItemSprite();
        }
    }

    private Sprite GetItemSprite()
    {

        SpriteRenderer result = item.GetComponent<SpriteRenderer>();
        Transform child = item.transform.GetChild(0);
        while (result == null)
        {
            if (child == null) return null;
            result = child.GetComponent<SpriteRenderer>();
            child = child.GetChild(0);
        }
        return result.sprite;
    }

    public void Interact()
    {
        FindObjectOfType<PlayerInventory>().AddWeapon(item);
        FindObjectOfType<PlayerInteraction>().UnRegisterInteractable(gameObject);
        canInteract = false;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void PopAndDie()
    {
        Destroy(Instantiate(popVFX, transform.position, Quaternion.identity), 2f);

        canInteract = false;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}