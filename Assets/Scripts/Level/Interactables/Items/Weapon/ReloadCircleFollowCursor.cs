using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ReloadCircleFollowCursor : MonoBehaviour
{
    PlayerInput playerInput;
    [SerializeField] Crosshair crosshair;
    [SerializeField] Transform reloadCanvas;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        reloadCanvas = GameObject.Find("ReloadCanvasTransform").GetComponent<Transform>();
        crosshair = FindObjectOfType<Crosshair>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 mousePos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);

        if (playerInput.currentControlScheme != "Keyboard and mouse")
        {
            if(reloadCanvas != null && crosshair != null)
            {
            reloadCanvas.position =
                crosshair.transform.position;
            }
        }
        else
        {
            reloadCanvas.position = mousePos;
        }
    }
}
