using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAudio : MonoBehaviour
{
    AudioSource catAnimationAudio;
    [SerializeField] AudioClip[] footsteps;

    // Start is called before the first frame update
    void Start()
    {
        catAnimationAudio = GetComponent<AudioSource>();
    }

    void PlayFootstep()
    {
        catAnimationAudio.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
    }
}
