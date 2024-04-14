using UnityEngine;
using UnityEngine.InputSystem;

public class GunRotation : MonoBehaviour
{
    [SerializeField] Transform barrelExitPoint;

    PlayerInput playerInput;
    Crosshair crosshair;
    Vector3 mousePos;
    Vector3 aimTarget;
    Vector3 gunPivotToExit;
    float angle;

    private void Start()
    {
        playerInput = GameSession.Instance.playerInput;
        crosshair = GameSession.Instance.crosshair;
    }

    // Update is called once per frame
    void Update()
    {
        gunPivotToExit = transform.localPosition - transform.InverseTransformPoint(barrelExitPoint.position);
        CalculateAimTarget();
        CalculateOffset();
        SetRotation();
        FlipGun();
    }

    void CalculateAimTarget()
    {
        if (playerInput.currentControlScheme != "Keyboard and mouse")
        {
            UnityEngine.Cursor.visible = false;
            crosshair.gameObject.SetActive(true);
            aimTarget = crosshair.transform.position;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            crosshair.gameObject.SetActive(false);
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimTarget = mousePos;
            aimTarget.z = 0;
        }
    }

    void CalculateOffset()
    {
        Vector3 gunPivotToMouse = transform.localPosition - transform.InverseTransformPoint(mousePos);
        angle = Mathf.Atan(gunPivotToExit.y / gunPivotToMouse.x) * Mathf.Rad2Deg;
    }

    void SetRotation()
    {

        Vector3 lookRotation = transform.position - aimTarget;
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
        if (playerInput.currentControlScheme != "Keyboard and mouse")
        {
            GunFlipController();
        }
        else
        {
            GunFlipMouse();
        }

        barrelExitPoint.localRotation = Quaternion.identity;
    }

    void GunFlipMouse()
    {
        if (mousePos.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void GunFlipController()
    {
        if (crosshair.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
