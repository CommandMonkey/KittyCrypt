using UnityEngine;

public class CatEchoSpawner : MonoBehaviour
{   
    [Header("How denset the trail is")] 
    [SerializeField] public float startTimeBetweenSpawns;

    private float usedTimeBetweenSpawns;

    public bool dashing = false;

    public GameObject echoPrefab;

    void FixedUpdate()
    {
        if (usedTimeBetweenSpawns <= 0 && dashing == true)
        {
            GameObject echo = Instantiate(echoPrefab, transform.position, transform.rotation);
            usedTimeBetweenSpawns = startTimeBetweenSpawns;
        }
        else
        {
            usedTimeBetweenSpawns -= Time.deltaTime;
        }           
    }
    public void StrartingDashTrail(Quaternion rotaion) 
    { dashing = true; transform.rotation = rotaion; }
    public void StopDashTrail() 
    { dashing = false; }
}
 