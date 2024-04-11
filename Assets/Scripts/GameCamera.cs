using Cinemachine;
using UnityEngine;

public class GameCamera : MonoBehaviour
{

    // Cached References
    CinemachineVirtualCamera virtualCamera;

    [SerializeField] Transform primaryTarget;
    [SerializeField] Transform secondaryTarget;

    bool onlyPrimary = false;

    private void Start()
    {
        virtualCamera = transform.parent.GetComponentInChildren<CinemachineVirtualCamera>();

        if (secondaryTarget == null) onlyPrimary = true;
    }

    private void Update()
    {
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

    public void DoCameraShake()
    {
        virtualCamera.GetComponent<Animator>().SetTrigger("CameraShake");
    }

    public void focusOnBoss()
    {
        SetPrimaryTarget(GameObject.FindGameObjectWithTag("Boss").transform);
    }
    
    public void focusOnBossAndPlayer()
    {
        SetSecondaryTarget(GameObject.FindGameObjectWithTag("Boss").transform);
        SetPrimaryTarget(FindObjectOfType<Player>().transform);
    }
}
