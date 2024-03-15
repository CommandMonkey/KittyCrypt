using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBehaviour : MonoBehaviour
{
    private void Awake()
    {
        SingletonBehaviour[] listOfSingletons = FindObjectsOfType<SingletonBehaviour>();

        // if more than one SingletonBehaviours exists, die
        if (listOfSingletons.Length > 1)
        {
            gameObject.SetActive(false);

            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
