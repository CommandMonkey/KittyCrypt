using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float BlastRadius = 3f;
    [SerializeField] float explotionDamage = 70f;
    [SerializeField] LayerMask layers;
   
    // Start is called before the first frame update
    void Start()
    {
        

        ContactFilter2D contactFilter = new ContactFilter2D()
        {
            layerMask = ~layers
        };

        Collider2D[] collider2Ds = new Collider2D[60];
        int colliderCount = Physics2D.OverlapCircle(transform.position, BlastRadius, contactFilter, collider2Ds);

        if (colliderCount > 0)
        {
            foreach (Collider2D collider in collider2Ds) 
            {
               if (collider != null && collider.CompareTag("Enemy"))
                {
                    collider.gameObject.GetComponent<Enemy>().TakeDamage(explotionDamage);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
