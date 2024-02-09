using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mace : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;

    public Transform circleOrgin;
    public float radius;


    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        // Atan2 gives us an angle in radience
        // Rad2Deg transfers the radience to degrees
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        if (rotZ > 10f)
        {
      
        }
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = circleOrgin == null ? Vector3.zero : circleOrgin.position;
        Gizmos.DrawWireSphere(position, radius);
    }
}


