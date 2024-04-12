using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class catEcho : MonoBehaviour
{
    public float timeBetweenSpawns;
    public float startTimeBetweenSpawns;

    public GameObject echo;

    private void Update()
    {
        if (timeBetweenSpawns <= 0)
        {
            //spawngame obj
            Instantiate(echo, transform.position, Quaternion.identity);
            timeBetweenSpawns = startTimeBetweenSpawns;
        }
        else
        {
            timeBetweenSpawns -= Time.deltaTime;
        }
    }



}
