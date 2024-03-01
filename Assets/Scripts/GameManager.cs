using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SoundManager soundManager { get; private set; }
    public Camera mainCamera { get; private set; }

    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        mainCamera = Camera.main;
    }
}
