using UnityEngine;

public class HatOnEnemy : MonoBehaviour
{
    [SerializeField] GameObject[] hats;

    // Start is called before the first frame update
    void Start()
    {
        GameObject decidedHat = hats[Random.Range(0, hats.Length)];

        if (decidedHat != null)
        {
            decidedHat.SetActive(true);
        }
    }
}
