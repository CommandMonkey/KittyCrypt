using UnityEngine;

public class TriggerInteractBase : MonoBehaviour, IInteractable
{
    public GameObject player { get; set; }
    public bool caninteract { get; set; }
    public string interactPrompt { get; set; }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        player.GetComponent<Player>().onInteract.AddListener(OnInteract);
    }

    void OnInteract()
    {
        if (caninteract)
        {
            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            caninteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            caninteract = false;
        }
    }


    public virtual void Interact() {}
}
