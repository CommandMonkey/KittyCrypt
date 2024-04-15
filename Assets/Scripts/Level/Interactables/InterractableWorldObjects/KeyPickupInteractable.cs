
using UnityEngine;

public class KeyPickupInteractable : MonoBehaviour, IInteractable
{
    public GameObject replacementPrefab;

    public string interactPrompt { get; set; }
    public bool canInteract { get; set; }

    Player player;

    private void Start()
    {
        player = GameSession.Instance.player;

        interactPrompt = "Rat Key";
        canInteract = true;
    }

    public void Interact()
    {
        gameObject.SetActive(false);
        FollowKey key = Instantiate(replacementPrefab, transform.position, transform.rotation).GetComponent<FollowKey>();
        key.target = player.transform;

        player.hasKey = true;
    }

}

