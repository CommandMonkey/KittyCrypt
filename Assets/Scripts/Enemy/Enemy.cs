
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy stats")]
    [SerializeField, Range(0, 10)] protected float speed = 5f;
    [SerializeField, Range(0, 1000)] protected float health = 1f;
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

    protected float maxHealth;
    protected bool lineOfSight = true;
    protected Vector3 targetPosition = Vector3.zero;

    protected GameSession gameSession;
    protected Animator animator;
    protected Rigidbody2D rigidBody2D;
    protected Transform target;
    protected Player player;

    bool isDead = false;


    private void Start()
    {
        gameSession = GameSession.Instance;
        animator = GetComponentInChildren<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        player = gameSession.player;
        target = player.transform;

        maxHealth = health;

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
        health -= damage;
        if (!isDead && health <= 0)
        {
            gameSession.onEnemyKill.Invoke();
            Die();
        }
        OnDamageTaken();
    }

    protected virtual void Die()
    {
        PlayDeathVFX();
        Destroy(gameObject);
    }

    void PlayHurtSFX()
    {
        GameObject sfxInstance = Instantiate(enemyHitAudio);
        Destroy(sfxInstance, 2f);
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

    protected virtual void OnDamageTaken() { }
    protected virtual void EnemyStart() { }
}