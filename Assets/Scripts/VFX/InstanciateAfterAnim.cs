using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanciateAfterAnim : MonoBehaviour
{
    Transform content;

    void Start()
    {
        content = transform.GetChild(0);
    }

    public void OnAnimDone()
    {
        content.SetParent(transform.parent);
        Destroy(gameObject);
    }
}
