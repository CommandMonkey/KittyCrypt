using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatBossGunLine : MonoBehaviour
{
    LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();    

        StartCoroutine(IncreaseWidth());
    }


    IEnumerator IncreaseWidth()
    {
        for (int i = 0; i < 5; i++)
        {
            lineRenderer.startWidth += .01f;
            lineRenderer.endWidth += .025f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return StartCoroutine(DecreaseWidth());
    }

    IEnumerator DecreaseWidth()
    {
        for (int i = 0; i < 5; i++)
        {
            lineRenderer.startWidth -= .01f;
            lineRenderer.endWidth -= .025f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
