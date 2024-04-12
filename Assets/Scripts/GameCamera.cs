using Cinemachine;
using UnityEngine;

public class GameCamera : MonoBehaviour
{

    // Cached References
    

    [SerializeField] Transform primaryTarget;
    [SerializeField] Transform secondaryTarget;

    bool onlyPrimary = false;
    CinemachineVirtualCamera virtualCamera;
    Animator camAnimator;


    private void Start()
    {
        virtualCamera = transform.parent.GetComponentInChildren<CinemachineVirtualCamera>();
        camAnimator = virtualCamera.GetComponent<Animator>();

        if (secondaryTarget == null) onlyPrimary = true;
    }

    private void Update()
    {
        if (secondaryTarget == null) onlyPrimary = true;
        if (onlyPrimary)
            transform.position = primaryTarget.position;
        else
            transform.position = primaryTarget.position + (secondaryTarget.position - primaryTarget.position) / 2;
    }

    public void SetPrimaryTarget(Transform transform)
    { 
        primaryTarget = transform;
    }

    public void SetSecondaryTarget(Transform transform)
    {
        if (transform != null)
            secondaryTarget = transform;
        else
            secondaryTarget = null;
        onlyPrimary = false;
    }

    public void DoCameraShake(float duration = 0.1f)
    {
        camAnimator.SetBool("cameraShake", true);
        Invoke("StopCameraShake", duration);

    }
    private void StopCameraShake()
    {
        camAnimator.SetBool("cameraShake", false);
    }

    //public void focusOnBoss()
    //{
    //    SetPrimaryTarget(GameObject.FindGameObjectWithTag("Boss").transform);
    //}
    
    //public void focusOnBossAndPlayer()
    //{
    //    SetSecondaryTarget(GameObject.FindGameObjectWithTag("Boss").transform);
    //    SetPrimaryTarget(FindObjectOfType<Player>().transform);
    //}

}
