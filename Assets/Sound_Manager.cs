using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sound_Manager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        // Set the slider's value to the initial music volume
        volumeSlider.value = musicSource.volume;
    }

    public void ChangeMusicVolume()
    {
        if (musicSource != null)
        {
            musicSource.volume = volumeSlider.value;
            Debug.Log("New music volume: " + volumeSlider.value);
        }
    }
}
