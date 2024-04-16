using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] Image[] heartsAmount;
    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite damagedHeart;

    int playerHealth;
    [SerializeField] Player player; 

    // Start is called before the first frame update
    void Start()
    {
        player = GameSession.Instance.player;
        if (player != null)
        {
            playerHealth = player.GetHealth();
        }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            playerHealth = player.GetHealth();
        }
        catch
        {
            player = GameSession.Instance.player;
        }


        for (int i = 0; i < heartsAmount.Length; i++) 
        {
            if (i < playerHealth)
            {
                heartsAmount[i].sprite = fullHeart;
            }
            else
            {
                heartsAmount[i].sprite = damagedHeart;
            }
        }
    }
}
