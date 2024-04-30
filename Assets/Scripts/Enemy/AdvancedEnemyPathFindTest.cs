using UnityEngine;

public class AdvancedEnemyPathFindTest : MonoBehaviour
{

    [SerializeField] Transform target;


    private void Update()
    {

        Shootray();
    }

    void Shootray()
    {
        Vector2 direction = target.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
        Debug.DrawRay(transform.position, hit.point, Color.blue);
        if (hit.collider != null)
        {
            Debug.Log("BINGUS D:"); 
        }       
        else
        {
            Debug.Log("Spoingus :D");
        }
        


    }



}
