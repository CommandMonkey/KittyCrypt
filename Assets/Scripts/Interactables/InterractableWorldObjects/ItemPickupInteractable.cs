using UnityEngine;

public class ItemPickupInteractable : MonoBehaviour, IInteractable
{
    public GameObject item;
    public string interactPrompt { get; set; }
    public bool canInteract { get; set; } = true;

    [SerializeField] GameObject popVFX;

    private void Start()
    {
        interactPrompt = "E to pickup " + item.name;
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