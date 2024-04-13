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
    Player player; 

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        playerHealth = player.GetHealth();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            playerHealth = player.GetHealth();
            Debug.Log(playerHealth);
        }
        catch
        {
            Debug.Log("dont have ref to player, fetching one");
            player = FindObjectOfType<Player>();
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
