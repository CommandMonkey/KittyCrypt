
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] public string itemName = "Item";
    [SerializeField] public Sprite hotbarImage;
    internal virtual void Activate()
    {
        
    }

    internal virtual void DeActivate()
    {
        
    }

    internal virtual void OnFire() { }
    internal virtual void OnReload() { }
}
