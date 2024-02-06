using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class GunRotation : MonoBehaviour
{
    [SerializeField] Transform barrelExitPoint;

    Vector3 mousePos;
    Vector3 gunPivotToExit;
    float angle;

    private void Start()
    {
        gunPivotToExit = transform.localPosition - transform.InverseTransformPoint(barrelExitPoint.position);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMousePos();
        CalculateOffset();
        SetRotation();
    }

    void CalculateMousePos()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
    }

    void CalculateOffset()
    {
        Vector3 gunPivotToMouse = transform.localPosition - transform.InverseTransformPoint(mousePos);
        angle = Mathf.Atan(gunPivotToExit.y / gunPivotToMouse.x) * Mathf.Rad2Deg;
    }

    void SetRotation()
    {
        Vector3 lookRotation = transform.position - mousePos;
        Quaternion pivotToMouseRotation = Quaternion.FromToRotation(Vector3.left, lookRotation);
        Vector3 eulerAngles = pivotToMouseRotation.eulerAngles;
        eulerAngles.z -= angle;

        transform.rotation = Quaternion.Euler(eulerAngles);
    }
}
