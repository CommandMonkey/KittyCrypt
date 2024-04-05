
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy stats")]
    [SerializeField, Range(0, 10)] protected float speed = 5f;
    [SerializeField, Range(0, 1000)] protected float hp = 1f;
    [SerializeField, Range(0, 100)] protected int enemyDMG = 1;
    [Header("The Lower the number the faster the attack")]
    [SerializeField, Range(0.1f, 3)] protected float attackSpeed = 0.1f;
    [Header("VFX")]
    [SerializeField] protected GameObject BloodSplatVFX;
    [SerializeField] protected GameObject BloodStainVFX;
    [Header("SFX")]
    [SerializeField] protected GameObject enemyHitAudio;
    [Header("Other")]

    public LayerMask obsticleLayer;

    protected Transform target;
    protected bool lineOfSight = true;
    protected Vector3 playerPosition = Vector3.zero;

    protected void ShootLineOfSightRay()
    {
        Vector2 direction = target.position - transform.position;
        float distance = direction.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obsticleLayer);

        if (hit.collider != null)
        {
            lineOfSight = false;
        }
        else
        {
            lineOfSight = true;
            playerPosition = target.position;
        }
    }
}