using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatBossBehaviour : MonoBehaviour
{
    [Header("General")]
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float timeBetweenAttacks = 4f;
    [SerializeField] LayerMask obsticleLayer;
    [Header("Attack - shoot")]
    [SerializeField] float shootAimTime;
    [SerializeField] GameObject aimLineVFX;
    [SerializeField] GameObject shootLineVFX;

    enum RatState
    {
        paused,
        idle,
        attacking,
        dead
    }

    RatState state = RatState.paused;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void StartBoss()
    {
        state = RatState.idle;
        StartCoroutine(BossAttackLoopRoutine());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (state == RatState.paused) return;
        
    }

    IEnumerator BossAttackLoopRoutine()
    {
        while (state != RatState.dead)
        {
            // 3 shoot attacks then one rat spawn

            StartCoroutine(ShootAttackRoutine());
            yield return new WaitForSeconds(timeBetweenAttacks);
            StartCoroutine(ShootAttackRoutine());
            yield return new WaitForSeconds(timeBetweenAttacks);
            StartCoroutine(ShootAttackRoutine());
            yield return new WaitForSeconds(timeBetweenAttacks);

            StartCoroutine(SpawnRatAttackRoutine());
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    IEnumerator ShootAttackRoutine()
    {
        Vector3 shootPos = player.PredictFuturePosition(shootAimTime);

        AimLineVFX(shootPos);
        yield return new WaitForSeconds(shootAimTime);
        
        RaycastHit2D rayHit = Physics2D.Raycast(
            transform.position, 
            shootPos - transform.position, 
            30f, 
            obsticleLayer
        );
        
    }

    void AimLineVFX(Vector3 target)
    {
        Vector3 toTarget = target - transform.position;

        LineRenderer line = Instantiate(aimLineVFX).GetComponent<LineRenderer>();
        Destroy(line.gameObject);

        line.SetPosition(0, transform.position);
        line.SetPosition(0, toTarget*10);   

    }

    IEnumerator SpawnRatAttackRoutine()
    {
        yield break;
    }
}
