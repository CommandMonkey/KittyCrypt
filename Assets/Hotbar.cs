using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    PlayerInventory inventory;
    [SerializeField] Image[] hotbarSpriteRenderer;
    [SerializeField] Sprite[] gunIcons;
    
    // Start is called before the first frame update
    void Start()
    {
        hotbarSpriteRenderer = GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < gunIcons.Length; i++)
        {
            hotbarSpriteRenderer[i].sprite = gunIcons[i];
            if (gunIcons[i] != null)
            {
                gunIcons = inventory.itemInventory[i].GetComponentsInChildren<Sprite>();
            }
        }
    }
}




