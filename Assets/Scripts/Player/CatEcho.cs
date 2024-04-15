using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatEcho : MonoBehaviour
{
    [SerializeField] float trailsTimeInThisWorld = 0.45f;
    void Start()
    {
        StartCoroutine(DelayedDestroyRoutine());
    }
    IEnumerator DelayedDestroyRoutine()
    {
        yield return new WaitForSeconds(trailsTimeInThisWorld);
        Destroy(gameObject);
    }
}