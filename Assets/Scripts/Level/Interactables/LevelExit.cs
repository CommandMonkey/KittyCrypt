using UnityEngine;

public class LevelExit : MonoBehaviour
{
    Transform player;
    GameSession gameSession;
    
    private void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject.transform;
            
            gameSession.NextLevelLoad();
        } 
    }
    
    
}