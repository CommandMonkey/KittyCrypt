using UnityEngine;
using UnityEngine.UIElements;

public class GunRotation : MonoBehaviour
{
    [SerializeField] Transform barrelExitPoint;

    Vector3 mousePos;
    Vector3 gunPivotToExit;
    float angle;

    // Update is called once per frame
    void Update()
    {
        Debug.Log(transform.localEulerAngles);
        gunPivotToExit = transform.localPosition - transform.InverseTransformPoint(barrelExitPoint.position);
        CalculateMousePos();
        CalculateOffset();
        FlipGun();
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
        if (transform.localScale.x < 0)
        {
            mousePos = -mousePos;
            angle = -angle;
        } //angle might have something to do with bug when not at 0 0

        Vector3 lookRotation = transform.position - mousePos;
        Quaternion pivotToMouseRotation = Quaternion.FromToRotation(Vector3.left, lookRotation);
        Vector3 eulerAngles = pivotToMouseRotation.eulerAngles;
        eulerAngles.z -= angle;

        transform.rotation = Quaternion.Euler(eulerAngles);
    }

    void FlipGun()
    {
        float currentScaleX = transform.localScale.x;

        if (Mathf.Clamp(transform.localEulerAngles.z, 105, 255) == transform.localEulerAngles.z)
        {
            transform.localScale = new Vector3(-currentScaleX, transform.localScale.y, transform.localScale.z);
        }
    }
}
