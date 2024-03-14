using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public SoundManager soundManager { get; private set; }
    public Camera mainCamera { get; private set; }
    public Transform player;

    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        mainCamera = Camera.main;
    }
}
