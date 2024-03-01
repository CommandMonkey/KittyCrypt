using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pickup : MonoBehaviour
{
    public PickupType type;
    public GameObject content;
}

public enum PickupType
{
    Weapon
}
