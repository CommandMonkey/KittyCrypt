using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mace : MonoBehaviour
{
    private enemy_Test enemyT;
    private AnimationEvent Animation;
    public Animator Animatorn;
    [SerializeField] private float delay = 0.3f;
    [SerializeField] private bool AttackBlock;

    public Transform circleOrgin;
    public float radius;


    private void Start()
    {
        enemyT = FindObjectOfType<enemy_Test>();
        Animation = FindObjectOfType<AnimationEvent>();
    }

    void OnFire()
    {
             if (AttackBlock)
            {
                return;
            }
            Animatorn.SetTrigger("Meele Attack");
            AttackBlock = true;
            StartCoroutine(DelayAttack());
    }


    public IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(delay);
        AttackBlock = false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = circleOrgin == null ? Vector3.zero : circleOrgin.position;
        Gizmos.DrawWireSphere(position, radius);
    }


    public void DetectColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrgin.position, radius))
        {
           if (enemyT = collider.GetComponent<enemy_Test>())
           {
                enemyT.TakeDamage();
           }

        }
    }
}


