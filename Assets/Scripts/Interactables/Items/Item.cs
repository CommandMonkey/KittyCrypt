
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] public string itemName = "Item";
    [SerializeField] public Sprite hotbarImage;
    public virtual void Activate()
    {
        
    }

    public virtual void DeActivate()
    {
        
    }
}
