using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineManager : MonoBehaviour
{
    Vector3[] linePositions = new Vector3[2];
    LineRenderer gunLineRenderer;
    GunFire gunFire;

    // Start is called before the first frame update
    void Start()
    {
        gunLineRenderer = GetComponent<LineRenderer>();
        gunFire = FindObjectOfType<GunFire>();


        linePositions[0] = gunFire.transform.position;
        linePositions[1] = gunFire.GetBulletHitPoint();

        gunLineRenderer.positionCount = linePositions.Length;
        gunLineRenderer.SetPositions(linePositions);
    }
}
