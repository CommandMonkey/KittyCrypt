using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour 
{
    [SerializeField] AudioClip exploringTheme;
    [SerializeField] AudioClip ratBossTheme;
    [SerializeField] float fadeDuration = 2f; // Duration of fade (in seconds)

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayExploringTheme(false);
    }

    public void PlayExploringTheme(bool fade = true)
    {
        if (fade)
            StartCoroutine(FadeAndPlayTheme(exploringTheme));
        else
        {
            audioSource.clip = exploringTheme;
            audioSource.Play();
        }
    }

    public void PlayRatBossTheme(bool fade = true)
    {
        if (fade)
            StartCoroutine(FadeAndPlayTheme(ratBossTheme));
        else
        {
            audioSource.clip = ratBossTheme;
            audioSource.Play();
        }
    }

    private IEnumerator FadeAndPlayTheme(AudioClip theme)
    {
        float startVolume = audioSource.volume;
        float targetVolume = 0f; // Fade out

        // Gradually decrease volume
        while (audioSource.volume > targetVolume)
        {
            audioSource.volume -= Time.deltaTime / fadeDuration;
            yield return null;
        }

        // Change the theme
        audioSource.clip = theme;
        audioSource.Play();

        targetVolume = startVolume; // Fade in

        // Gradually increase volume
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

    public void StopMusic()
    {
        StartCoroutine(FadeAndPlayTheme(null));
    }

}