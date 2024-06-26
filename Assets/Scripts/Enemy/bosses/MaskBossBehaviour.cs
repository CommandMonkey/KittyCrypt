
using System;
using System.Collections;
using UnityEngine;

public class MaskBossBehaviour : Boss
{
    // /// CONFIGURABLE VARIABLES /// //
    [Header("Orbit Attack")]
    [SerializeField] float orbitDistance = 3f; // Distance from the player
    [SerializeField] float orbitSpeed = 5f; // Speed of orbit
    [SerializeField] float orbitFollowWeight = 0.6f;
    [SerializeField] float lungeInterval = 2f; // Time between lunges
    [SerializeField] float lungeSpeed = 10f; // Speed of the lunge

    [Header("Bullet Hell Attack")]
    [SerializeField] float bulletHellDuration;
    [SerializeField] Transform bulletSpawn;
    [SerializeField] float bulletSpawnWidth;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float hellBulletFrequency;
    [SerializeField] float hellBulletSpeed;


    // /// PRIVATE VARIABLES /// //

    // Bullet Hell Attack



    // Stagger (Damage Phase)



    // Orbit / Lunge Attack
    private GameObject orbitTarget;
    private float orbitAngle; // Current angle in the orbit
    private float lungeTimer; // Timer for lunging
    private Vector3 lungeTarget;




    internal override void StartBoss()
    {
        orbitTarget = new GameObject();
        lungeTimer = lungeInterval; // Initialize lunge timer
    }



    private enum State
    {
        None,  
        BulletHell,
        Staggered,
        Orbiting,
        Lunging
    }

    private State currentState = State.Orbiting;

    protected new void Start()
    {
        base.Start();

        StartBoss();
    }

    void Update()
    {
        if (currentState == State.None) return;

        switch (currentState)
        {
            case State.BulletHell:

                break;
            case State.Staggered:

                break;
            case State.Orbiting:
                OrbitPlayer();
                break;
            case State.Lunging:
                LungeAtPlayer();
                break;
        }

        UpdateOrbitTargetPos();
    }

    IEnumerator BulletHellRoutine()
    {
        float startTime = Time.time;

        while (true)
        {
            SpawnBullet();

            if (Time.time - startTime > bulletHellDuration) 
            {
                StopBulletHell();
                yield break;
            }

        }

        void StopBulletHell()
        {

        }
    }

    private void SpawnBullet()
    {
        Vector2 spawnPos = new Vector2(UnityEngine.Random.Range(-bulletSpawnWidth, bulletSpawnWidth), bulletSpawn.position.y);
        GameObject bulletInstance = Instantiate(bulletPrefab, spawnPos, Quaternion.Euler(Vector3.forward*90));
    }



    // Orbit / Lunge Attack
    private void UpdateOrbitTargetPos()
    {
        orbitTarget.transform.position = Vector3.Lerp(orbitTarget.transform.position, player.transform.position, orbitFollowWeight);
    }

    void StartOrbit()
    {
        // Set Orbit Angle based on current position relative to the player
        Vector2 direction = transform.position - player.transform.position;
        float angleInRadians = Mathf.Atan2(direction.y, direction.x);
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
        orbitAngle = angleInDegrees;

        currentState = State.Orbiting;
    }


    void OrbitPlayer()
    {
        orbitAngle += orbitSpeed * Time.deltaTime;
        if (orbitAngle > 360f) orbitAngle -= 360f;

        Vector3 offset = new Vector3(Mathf.Cos(orbitAngle), Mathf.Sin(orbitAngle)) * orbitDistance;
        transform.position = orbitTarget.transform.position + offset;

        lungeTimer -= Time.deltaTime;
        if (lungeTimer <= 0)
        {
            StartLunge();

            // Set next orbit Angle
        }
    }

    private void StartLunge()
    {
        lungeTimer = lungeInterval;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        lungeTarget = transform.position + direction * orbitDistance * 2;
        currentState = State.Lunging;
    }

    void LungeAtPlayer()
    {
        Vector3 direction = (lungeTarget - transform.position).normalized;
        transform.position += direction * lungeSpeed * Time.deltaTime;

        //if (!lungePassedPlayer && Vector3.Distance(transform.position, player.transform.position) < 0.5f) // Close enough to the player
        //{
        //    lungePassedPlayer = true;
        //}
        if (Vector3.Distance(transform.position, lungeTarget) <= 0.5)
        {
            StartOrbit();
            //orbitAngle += 180;
            //orbitAngle -= orbitSpeed * Time.deltaTime;
            //currentState  = State.Orbiting;
        }
    }
}
