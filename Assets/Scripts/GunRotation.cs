using UnityEngine;

public class GunRotation : MonoBehaviour
{
    [SerializeField] Transform barrelExitPoint;

    Vector3 mousePos;
    Vector3 positiveMouseDir;
    Vector3 gunPivotToExit;
    float angle;

    private void Start()
    {
        positiveMouseDir = mousePos;
    }

    // Update is called once per frame
    void Update()
    {

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
        }

        Vector3 lookRotation = transform.position - mousePos;
        Quaternion pivotToMouseRotation = Quaternion.FromToRotation(Vector3.left, lookRotation);
        Vector3 eulerAngles = pivotToMouseRotation.eulerAngles;
        eulerAngles.z -= angle;

        transform.rotation = Quaternion.Euler(eulerAngles);
    }

    void FlipGun()
    {
        float currentScaleX = transform.localScale.x;

        if (Mathf.Clamp(transform.rotation.eulerAngles.z, 105, 255) == transform.rotation.eulerAngles.z)
        {
            transform.localScale = new Vector3(-currentScaleX, transform.localScale.y, transform.localScale.z);
        }
        Debug.Log(transform.rotation.eulerAngles.z.ToString());
    }
}
