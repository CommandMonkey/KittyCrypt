using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Load Settings")]
    [SerializeField] bool spawnRooms = true;


    public SoundManager soundManager { get; private set; }
    public Camera mainCamera { get; private set; }
    public Player player;
    public bool gamePaused = false;

    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        player = FindObjectOfType<Player>();
        mainCamera = Camera.main;

        if (spawnRooms) StartLevelGeneration();
    }


    void StartLevelGeneration()
    {

    }

}
