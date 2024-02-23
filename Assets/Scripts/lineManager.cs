using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineManager : MonoBehaviour
{
    Vector3[] linePositions = new Vector3[2];
    LineRenderer gunLineRenderer;
    GunFire gunFire;
    IEnumerator coroutine;

    // Start is called before the first frame update
    void Start()
    {
        gunLineRenderer = GetComponent<LineRenderer>();
        gunFire = FindObjectOfType<GunFire>();


        linePositions[0] = gunFire.transform.position;
        linePositions[1] = gunFire.GetBulletHitPoint();

        gunLineRenderer.positionCount = linePositions.Length;
        gunLineRenderer.SetPositions(linePositions);

        coroutine = IncreaseWidth();

        StartCoroutine(coroutine);
    }

    IEnumerator IncreaseWidth()
    {
        for (int i = 0; i < 5; i++)
        {
            gunLineRenderer.startWidth += .01f;
            gunLineRenderer.endWidth += .02f;
            yield return new WaitForSeconds(0.01f);
        }
            yield return StartCoroutine(DecreaseWidth());
    }

    IEnumerator DecreaseWidth()
    {
        for (int i = 0; i < 5; i++)
        {
            gunLineRenderer.startWidth -= .01f;
            gunLineRenderer.endWidth -= .02f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
