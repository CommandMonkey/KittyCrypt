using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class enemy_Test : MonoBehaviour
{
    float health = 3f;
    float maxHealth = 3f;

    private void Start()
    {
        health = maxHealth;
    }


    public void TakeDamage()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }

    }
}

