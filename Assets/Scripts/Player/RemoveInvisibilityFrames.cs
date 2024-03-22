using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveInvisibilityFrames : MonoBehaviour
{
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void SetDamageTakenFalse()
    {
        player.SetDamageTakenFalse();
    }
}
