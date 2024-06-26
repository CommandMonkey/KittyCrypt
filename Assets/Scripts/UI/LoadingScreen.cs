using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoadingScreen : MonoBehaviour
{
    GameSession levelManager;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<GameSession>();
        animator = GetComponent<Animator>();

        animator.SetInteger("levelState", (int)GameSession.state);

        levelManager.OnNewState.AddListener(OnNewState);
    }

    private void OnNewState()
    {
        animator.SetInteger("levelState", (int)GameSession.state);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
