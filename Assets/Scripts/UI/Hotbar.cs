using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    PlayerInventory inventory;
    [SerializeField] Image[] hotbarSpriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        hotbarSpriteRenderer = GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < hotbarSpriteRenderer.Length; i++)
        {
            Item gunIcon = null;
            if (inventory.itemInventory[i] != null)
            {
                gunIcon = GameHelper.GetComponentInAllChildren<Item>(inventory.itemInventory[i].transform);
            }

            if (gunIcon != null) hotbarSpriteRenderer[i].sprite = gunIcon.hotbarImage;

        }
    }
}




