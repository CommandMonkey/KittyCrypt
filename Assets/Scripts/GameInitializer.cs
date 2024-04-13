using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] GameObject GameSessionPrefab;
    
    public void InstanciateGameSession()
    {
        Instantiate(GameSessionPrefab);
    }
}
