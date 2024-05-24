using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    PlayerInventory inventory;
    [SerializeField] Image[] hotbarSpriteRenderer;
    [SerializeField] List<Item> gunIcons;
    
    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        hotbarSpriteRenderer = GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i <= hotbarSpriteRenderer.Length; i++)
        {
            Debug.Log("Hello there this is i: " + i);
            if (inventory.itemInventory.Length > i)
            {
                gunIcons = GameHelper.GetComponentsInAllChildren<Item>(inventory.itemInventory[i].transform);
            }
            if (gunIcons.Count > i)
            {
                hotbarSpriteRenderer[i].sprite = gunIcons[i].hotbarImage;
            }
        }
    }
}




