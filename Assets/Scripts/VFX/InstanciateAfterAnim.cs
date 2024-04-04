using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanciateAfterAnim : MonoBehaviour
{
    [SerializeField] GameObject content;


    public void OnAnimDone()
    {
        Instantiate(content, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
