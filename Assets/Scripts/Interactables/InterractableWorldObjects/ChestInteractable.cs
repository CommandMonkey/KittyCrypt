using System.Collections.Generic;
using UnityEngine;

public class ChestInteractable : MonoBehaviour, IInteractable
{
    public string interactPrompt { get; set ; }
    public bool canInteract { get; set; } = true;

    [SerializeField] List<Transform> launchDirections;
    [SerializeField] float initialShootOutSpeed;
    [SerializeField] float gravityFactor;
    [SerializeField] Sprite itemSpawnBallSprite;
    [SerializeField] Sprite openChest;
    [SerializeField] GameObject pickUpPrefab;
    SpriteRenderer mySpriteRenderer;

    List<Rigidbody2D> gravityObjects = new List<Rigidbody2D>();
    List<ItemPickupInteractable> instancedItemPickups = new List<ItemPickupInteractable>();
    List<GameObject> itemsSpawned = new List<GameObject>();

    GameSession gameSession;

    Vector2 gravVector;

    // Start is called before the first frame update
    void Start()
    {
        gameSession = GameSession.Instance; 
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

            if (popAll)
            {
                foreach (ItemPickupInteractable obj in instancedItemPickups)
                {
                    if (obj != null)
                        obj.PopAndDie();
                }
                FindObjectOfType<PlayerInteraction>().UnRegisterInteractable(gameObject);
                Destroy(this);
            }

        }

    }
    private void FixedUpdate()
    {

        if (gravityObjects.Count < 1) 
            return;
        else
            foreach(Rigidbody2D obj in gravityObjects)
            {
                obj.velocity += gravVector;
                if (obj.position.y < transform.position.y)
                {
                    obj.velocity = Vector2.zero;
                    gravityObjects.Remove(obj);
                    spawnPickup(obj.position);
                    Destroy(obj.gameObject);
                    break;
                }
            }
    }

    private void spawnPickup(Vector2 position)
    {
        int tries = 0;

        GameObject item = null;
        while (item == null || itemsSpawned.Contains(item))
        {
            item = gameSession.levelSettings.GetRandomSpawnRoomItem();
            tries++;
            if (tries == 50) break;
        }
        itemsSpawned.Add(item);

        ItemPickupInteractable pickup = Instantiate(pickUpPrefab, position, Quaternion.identity).GetComponent<ItemPickupInteractable>();
        instancedItemPickups.Add(pickup);
    }

    public void Interact()
    {
        mySpriteRenderer.sprite = openChest;
        //Instantiate(pickUpPrefab, new Vector2(transform.position.x, transform.position.y + .5f), Quaternion.identity);
        canInteract = false;

        foreach (Transform obj in launchDirections)
        {
            LaunchObject(obj.localPosition);
        }
    }

    void LaunchObject(Vector2 direction)
    {
        GameObject obj = new GameObject();
        obj.transform.SetParent(transform, false);
        Rigidbody2D objRigidbody = obj.AddComponent<Rigidbody2D>();
        SpriteRenderer objRenderer = obj.AddComponent<SpriteRenderer>();
        objRenderer.sprite = itemSpawnBallSprite;

        obj.transform.position +=  new Vector3(0, 1f);

        objRigidbody.bodyType = RigidbodyType2D.Kinematic;
        //Rigidbody2D instance = Instantiate(obj, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        objRigidbody.velocity += direction.normalized * initialShootOutSpeed;

        gravityObjects.Add(objRigidbody);
    }
}
