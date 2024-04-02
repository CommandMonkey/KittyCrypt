using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoadingScreen : MonoBehaviour
{
    LevelManager levelManager;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        animator = GetComponent<Animator>();

        animator.SetInteger("levelState", (int)levelManager.state);

        levelManager.OnNewState.AddListener(OnNewState);
    }

    private void OnNewState()
    {
        animator.SetInteger("levelState", (int)levelManager.state);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
