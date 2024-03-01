using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pickup : MonoBehaviour
{
    public PickupType type;
    public GameObject content;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = content.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
    }
}

public enum PickupType
{
    Weapon
}
