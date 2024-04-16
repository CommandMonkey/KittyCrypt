using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ReloadCircleFollowCursor : MonoBehaviour
{
    PlayerInput playerInput;
    [SerializeField] Crosshair crosshair;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        crosshair = GameSession.Instance.crosshair;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);

        if (playerInput.currentControlScheme != "Keyboard and mouse")
        {
            transform.position = crosshair.transform.position;
        }
        else
        {
            transform.position = mousePos;
        }
    }
}
