using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodStainVFX : MonoBehaviour
{
    [SerializeField] float sizeIncrease = 0.1f;
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
                Destroy(gameObject);
                return;
            }
        }

        // if is not touching other splat
        activeInstances.Add(collider2d);
    }

    private void OnDestroy()
    {
        activeInstances.Remove(collider2d);
    }
}
