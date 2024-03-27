using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [Header("Load Settings")]
    public bool spawnRooms = true;
    public LevelState state = LevelState.Loading;

    [NonSerialized] public UnityEvent OnEnemyKill;

    public enum LevelState
    {
        Running,
        Paused,
        Loading,
    }
    public SoundManager soundManager { get; private set; }
    public Camera mainCamera { get; private set; }
    public Player player;
    public Transform enemyContainer;

    public bool gamePaused = false;


    private void Awake()
    {
        OnEnemyKill = new UnityEvent();
    }

    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        player = FindObjectOfType<Player>();
        mainCamera = Camera.main;
    }
}


