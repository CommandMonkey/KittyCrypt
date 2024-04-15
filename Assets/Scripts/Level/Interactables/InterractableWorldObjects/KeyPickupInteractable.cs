
using UnityEngine;

public class KeyPickupInteractable : MonoBehaviour
{
    public GameObject replacementPrefab;



    public void Interact()
    {
        gameObject.SetActive(false);
        Instantiate(replacementPrefab, transform.position, transform.rotation);
    }

}

