using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AudioButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [SerializeField] AudioClip buttonHoverSound;
    [SerializeField] AudioClip buttonClickSound;

    [SerializeField] AudioSource audioSource;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonHoverSound != null)
            audioSource.PlayOneShot(buttonHoverSound);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonClickSound != null)
            audioSource.PlayOneShot(buttonClickSound);
    }


}
