using UnityEngine;

public class GunRotation : MonoBehaviour
{
    [SerializeField] Transform barrelExitPoint;

    Vector3 mousePos;
    Vector3 gunPivotToExit;
    float angle;

    // Update is called once per frame
    void Update()
    {
        gunPivotToExit = transform.localPosition - transform.InverseTransformPoint(barrelExitPoint.position);
        CalculateMousePos();
        CalculateOffset();
        SetRotation();
        FlipGun();
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

        if (transform.localScale.y < 0)
        {
            eulerAngles.z += angle;
        }
        else
        {
            eulerAngles.z -= angle;
        }
        transform.rotation = Quaternion.Euler(eulerAngles);
    }

    void FlipGun()
    {
        //TODO make gun shoot right direction
        if (mousePos.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        barrelExitPoint.localRotation = Quaternion.identity;
    }
}
