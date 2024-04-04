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
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (secondaryTarget == null) onlyPrimary = true;
    }

    private void Update()
    {
        if (onlyPrimary)
            transform.position = primaryTarget.position;
        else
            transform.position = primaryTarget.position + (secondaryTarget.position - primaryTarget.position) / 2;
    }

    public void SetPrimaryTarget(ref Transform transform)
    {
        primaryTarget = transform;
    }

    public void SetSecondaryTarget(ref Transform transform)
    {
        secondaryTarget = transform;
        onlyPrimary = false;
    }
}
