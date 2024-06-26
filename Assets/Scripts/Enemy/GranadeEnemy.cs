using UnityEngine;

public class GranadeEnemy : Enemy
{
    [SerializeField] SpriteRenderer chargeUpGranadeSprite;
    [SerializeField] GameObject granadePrefab;
    [SerializeField] float granadeSpeed = 5;
   
    bool inCombat = false;
    [SerializeField] GranadeEnemyState state = GranadeEnemyState.idle; // Serialized for DEBUG

    Vector2 walkingTarget;

    enum GranadeEnemyState
    {
        idle, // set up walking                         / onDone => walking
        walking, // Walking                             / onDone => waitingToThrow
        waitingToThrow, // Set up and start throw       / onDone => Throwing
        throwing // Waiting while throwing, Cooldown    / onDone => idle
    }

    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        

    }
    
    // Update is called once per frame
    void Update()
    { 

        if (!inCombat)
        {
            if (lineOfSight) inCombat = true;
            return;
        }


        if (state == GranadeEnemyState.idle)
        {
            DetermineWalkingTarget();
            state = GranadeEnemyState.walking;
        }
        else if (state == GranadeEnemyState.walking)
        {
            if ((walkingTarget - (Vector2)transform.position).magnitude >= .1f)
            {
                state = GranadeEnemyState.waitingToThrow;
            }
        }
        else if (state == GranadeEnemyState.waitingToThrow)
        {
            chargeUpGranadeSprite.enabled = true;
        }
        else if (state == GranadeEnemyState.throwing)
        {

        }

        FlipSprite();
    }

    // Called by animation Event 
    public void OnGranadeThrow()
    {
        chargeUpGranadeSprite.enabled = false;
        GameObject _GranadeInstance = Instantiate(granadePrefab, chargeUpGranadeSprite.transform.position, Quaternion.identity);
        _GranadeInstance.GetComponent<Rigidbody2D>().velocity = (targetPosition - chargeUpGranadeSprite.transform.position) * granadeSpeed;

    }

    private void DetermineWalkingTarget()
    {



    }

    void FlipSprite()
    {   
        if (rigidBody2D.velocity.x > 0)
        { transform.rotation = Quaternion.Euler(0f, 180f, 0f); }
        else if (rigidBody2D.velocity.x < 0)
        { transform.rotation = Quaternion.Euler(0f, 0f, 0f); }
    }
}
