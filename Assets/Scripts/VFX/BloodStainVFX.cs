using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BloodStainVFX : MonoBehaviour
{
    [SerializeField] float sizeIncrease = 0.1f;
    [SerializeField] float maxSize = 7f;
    static List<Collider2D> activeInstances = new List<Collider2D>();

    Collider2D collider2d;


    private void Start()
    {
        // Check if colliding with other splat, if yes: Die, if no: live

        collider2d = GetComponent<Collider2D>();
        ContactFilter2D contactFilter = new ContactFilter2D()
        {
            layerMask = LayerMask.GetMask("Default"),
            useTriggers = true
        };
        Collider2D[] results = new Collider2D[10];
        collider2d.OverlapCollider(contactFilter, results);

        foreach (Collider2D col in results)
        {
            if (activeInstances.Contains(col))
            {
                col.transform.localScale += new Vector3(sizeIncrease, sizeIncrease, sizeIncrease);
                col.transform.localScale = Vector3.Min(col.transform.localScale, new Vector3(maxSize, maxSize, maxSize));
                Destroy(gameObject);
                return;
            }
        }

        // if is not touching other splat
        activeInstances.Add(collider2d);
        float randomAngle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, 0f, randomAngle);
    }

    private void OnDestroy()
    {
        activeInstances.Remove(collider2d);
    }
}
