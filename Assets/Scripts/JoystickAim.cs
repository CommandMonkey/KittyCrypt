using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickAim : MonoBehaviour
{
    public Player pM;
    public Transform GunGripper;
    public float aiming;
    // Start is called before the first frame update
    void Start()
    {
        pM = FindObjectOfType<Player>();
       // aiming = vaule.get();
    }

    // Update is called once per frame
    void Update()
    {
        Aim();
}
    void onAiming(InputValue value)
    {
        value.Get();
    }

    public void Aim()
    {
       // GunGripper.Transform.localEulerAngles = new Vector3(0f,0f,Mathf.Atan(onAiming) ;
    }
}
