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
    [SerializeField] float gravityFactor;
    [SerializeField] Sprite openChest;
    [SerializeField] GameObject pickUpPrefab;
    SpriteRenderer mySpriteRenderer;

    List<Rigidbody2D> gravityObjects;
    List<GameObject> instancedItemPickups;

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
        foreach (GameObject obj in instancedItemPickups) 
        {
            
        }

        if (gravityObjects.Count == 0) return;
        else
            foreach(Rigidbody2D obj in gravityObjects)
            {
                obj.velocity -= gravityFactor;
            }
    }

    public void Interact()
    {
        mySpriteRenderer.sprite = openChest;
        Instantiate(pickUpPrefab, new Vector2(transform.position.x, transform.position.y + .5f), Quaternion.identity);
        canInteract = false;
    }

    void LaunchObject()
    {
        
    }
}
