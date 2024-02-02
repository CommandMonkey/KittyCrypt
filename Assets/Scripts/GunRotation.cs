using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GunRotation : MonoBehaviour
{
    [SerializeField] Transform barrelExitPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 gunPivotToExit = transform.localPosition - transform.InverseTransformPoint(barrelExitPoint.position);
        Vector3 gunPivotToMouse = transform.localPosition - transform.InverseTransformPoint(mousePos);

        float angle = Mathf.Atan(gunPivotToExit.y / gunPivotToMouse.x) * Mathf.Rad2Deg;
        Debug.Log(angle);
        

        Vector3 lookRotation = transform.position - mousePos;
        Quaternion pivotToMouseRotation = Quaternion.FromToRotation(Vector3.left, lookRotation);
        Vector3 eulerAngles = pivotToMouseRotation.eulerAngles;
        eulerAngles.z -= angle;

        transform.rotation = Quaternion.Euler(eulerAngles);




    }
}
