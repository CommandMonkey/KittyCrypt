
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


    protected bool lineOfSight = true;
    protected Vector3 targetPosition = Vector3.zero;

    protected LevelManager levelManager;
    protected Animator animator;
    protected Rigidbody2D rigidBody2D;
    protected Transform target;
    protected Player player;


    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        animator = GetComponentInChildren<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        target = levelManager.player.transform;
        player = target.GetComponent<Player>();

        
        if (player == null) { Debug.Log("PLAYER FOUND"); }

        EnemyStart();
    }

    protected bool ShootLineOfSightRay()
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
            targetPosition = target.position;
        }
        return lineOfSight;
    }

    public void TakeDamage(float damage)
    {
        PlayHurtVFX();
        PlayHurtSFX();
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        levelManager.onEnemyKill.Invoke();
        PlayDeathVFX();
        Destroy(gameObject);
    }

    void PlayHurtSFX()
    {
        Instantiate(enemyHitAudio);
    }

    private void PlayHurtVFX()
    {
        animator.SetTrigger("WasHurt");
        Instantiate(BloodStainVFX, transform.position, Quaternion.identity);
    }

    void PlayDeathVFX()
    {
        GameObject Blood = Instantiate(BloodSplatVFX, transform.position, transform.rotation);
        Destroy(Blood, 1f);
    }

    protected virtual void EnemyStart() { }
}