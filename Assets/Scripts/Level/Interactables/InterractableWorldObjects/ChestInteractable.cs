using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.WSA;

public class ChestInteractable : MonoBehaviour, IInteractable
{
    public string interactPrompt { get; set ; }
    public bool canInteract { get; set; } = true;

    [SerializeField] List<Transform> launchDirections;
    [SerializeField] float initialShootOutSpeed;
    [SerializeField] float gravityFactor;
    [SerializeField] Sprite openChest;
    [SerializeField] GameObject pickUpPrefab;
    SpriteRenderer mySpriteRenderer;

    List<Rigidbody2D> gravityObjects = new List<Rigidbody2D>();
    List<ItemPickupInteractable> instancedItemPickups = new List<ItemPickupInteractable>();

    Vector2 gravVector;

    // Start is called before the first frame update
    void Start()
    {
        interactPrompt = "Chest";
        mySpriteRenderer = GetComponent<SpriteRenderer>();

        gravVector = new Vector2(0, gravityFactor);
    }

    private void Update()
    {
        if (instancedItemPickups.Count > 0)
        {
            bool popAll = false;
            foreach (ItemPickupInteractable obj in instancedItemPickups)
            {
                if (obj == null)
                {
                    popAll = true;
                }
            }

            if(popAll)
                foreach (ItemPickupInteractable obj in instancedItemPickups)
                {
                    obj.PopAndDie();
                }
        }


        if (gravityObjects.Count == 0) return;
        else
            foreach(Rigidbody2D obj in gravityObjects)
            {
                if (obj.position.y < transform.position.y)
                {
                    obj.velocity = Vector2.zero;
                    gravityObjects.Remove(obj);
                    break;
                }
                else
                    obj.velocity -= gravVector;
            }
    }

    public void Interact()
    {
        mySpriteRenderer.sprite = openChest;
        Instantiate(pickUpPrefab, new Vector2(transform.position.x, transform.position.y + .5f), Quaternion.identity);
        canInteract = false;

        foreach (Transform obj in launchDirections)
        {
            LaunchObject(obj.position);
        }
    }

    void LaunchObject(Vector2 direction)
    {
        GameObject obj = new GameObject();
        obj.AddComponent<Rigidbody2D>();

        Rigidbody2D instance = Instantiate(obj, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        instance.AddForce(direction * initialShootOutSpeed, ForceMode2D.Impulse);
        gravityObjects.Add(instance);
    }
}
