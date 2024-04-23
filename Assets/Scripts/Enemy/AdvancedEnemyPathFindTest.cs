using UnityEngine;

public class AdvancedEnemyPathFindTest : MonoBehaviour
{

    [SerializeField] Transform target;


    private void Update()
    {
        Vector2 direction = target.position - transform.position;
        float distance = direction.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance);
        if (hit.collider != null)
        {
            Debug.Log("BINGUS D:");
            Debug.DrawRay(transform.position, hit.point, Color.blue);
        }
        else
        {
            Debug.Log("Spoingus :D");
        }
        


    }



}
