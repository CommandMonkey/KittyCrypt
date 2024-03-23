using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Load Settings")]
    public bool spawnRooms = true;

    public SoundManager soundManager { get; private set; }
    public Camera mainCamera { get; private set; }
    public Player player;
    public Transform enemyContainer;

    public bool gamePaused = false;

    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        player = FindObjectOfType<Player>();
        mainCamera = Camera.main;
    }
}
