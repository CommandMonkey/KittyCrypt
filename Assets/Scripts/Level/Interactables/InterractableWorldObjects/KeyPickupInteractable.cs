using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class KeyPickupInteractable : MonoBehaviour
{
    public GameObject replacementPrefab;



    public void Interact()
    {
        gameObject.SetActive(false);
        Instantiate(replacementPrefab, transform.position, transform.rotation);
    }

}

