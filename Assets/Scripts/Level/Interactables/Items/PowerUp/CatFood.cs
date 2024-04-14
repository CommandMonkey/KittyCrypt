using UnityEngine;

public class CatFood : MonoBehaviour
{
    [SerializeField] GameObject pickupVFX;

    Player player;

    private void Start()
    {
        player = GameSession.Instance.player;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
