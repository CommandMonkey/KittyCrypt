using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatBossGunLine : MonoBehaviour
{
    [SerializeField] int sampleRate = 15;
    [SerializeField] float startWidth = 1f;
    public float timeToZero = 1f;

    [SerializeField] float decreaseFactor = 0;
    [SerializeField] float sampleWaitTime = 0;

    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();    

        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = startWidth;

        decreaseFactor = startWidth / sampleRate * timeToZero;
        sampleWaitTime = 1f / sampleRate;

        StartCoroutine(DecreaseWidth());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DecreaseWidth()
    {
        while (lineRenderer.endWidth > 0)
        {
            lineRenderer.startWidth -= decreaseFactor;
            lineRenderer.endWidth -= decreaseFactor;
            yield return new WaitForSeconds(sampleWaitTime);
        }
    }
}
