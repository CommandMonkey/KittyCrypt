using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour 
{
    [SerializeField] AudioClip exploringTheme;
    [SerializeField] AudioClip ratBossTheme;
    [SerializeField] AudioClip battleTheme;
    [SerializeField] AudioClip mainMenuTheme;
    [SerializeField] float fadeDuration = 2f; // Duration of fade (in seconds)

    AudioSource audioSource;

    float exploringThemePauseTime = 0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene().buildIndex != 0)
            PlayExploringTheme(false);
        else
            PlayMainMenuTheme(false);
    }

    private void PlayMainMenuTheme(bool fade)
    {
        StartTheme(mainMenuTheme, fade);
    }

    public void PlayExploringTheme(bool fade = true)
    {
        StartTheme(exploringTheme, fade);
    }

    public void PlayRatBossTheme(bool fade = true)
    {
        StartTheme(ratBossTheme, fade);
    }

    public void PlayBattleTheme(bool fade = true)
    {
        StartTheme(battleTheme, fade);
    }

    private void StartTheme(AudioClip theme, bool fade)
    {
        if (audioSource.clip == exploringTheme && theme != exploringTheme)
        {
            exploringThemePauseTime = audioSource.time;
        }

        if (fade)
            StartCoroutine(FadeAndPlayTheme(theme));
        else
        {
            audioSource.clip = theme;
            audioSource.Play();
        }

        if (theme == exploringTheme) 
        { 
            audioSource.time = exploringThemePauseTime;
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