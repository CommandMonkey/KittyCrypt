using System;
using System.Collections;
using UnityEngine;

public class CatFood : MonoBehaviour
{
    [SerializeField] GameObject pickupVFX;
    [SerializeField] float initialForceMagnitude;
    [SerializeField] float gravityMagnitude;
    [SerializeField] float timeUntilStop;
    [SerializeField] int healthIncrease = 1;

    Player player;
    Rigidbody2D rigidbody2;

    bool simulateGravity = false;
    Vector2 gravityVector;

    private void Start()
    {
        player = GameSession.Instance.Player;
        rigidbody2 = GetComponent<Rigidbody2D>();

        gravityVector = new Vector2(0, gravityMagnitude);

        StartCoroutine(DoMovementRoutine());
    }

    private void FixedUpdate()
    {
        if (simulateGravity) 
            rigidbody2.velocity += gravityVector;
    }

    private IEnumerator DoMovementRoutine()
    {
        Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-.3f, .3f), 1).normalized;
        rigidbody2.velocity += randomDirection * initialForceMagnitude;

        simulateGravity = true;

        yield return new WaitForSeconds(timeUntilStop);

        simulateGravity = false;
        rigidbody2.velocity = Vector2.zero;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag != "Player") { return; }
        player.AddHealth(healthIncrease);
        StopAllCoroutines();
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}