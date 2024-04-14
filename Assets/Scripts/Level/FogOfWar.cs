using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class FogOfWar : MonoBehaviour
    {
        [SerializeField] float timeToFade;

        SpriteRenderer spriteRenderer;

        float currentFadeTime = 0f;


        // Start is called before the first frame update
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void FadeAway()
        {
            StartCoroutine(FadeAwayRoutine());
        }

    private IEnumerator FadeAwayRoutine()
    {
        Color startColor = spriteRenderer.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Target color with alpha = 0

        while (currentFadeTime < timeToFade)
        {
            currentFadeTime += Time.deltaTime;

            // Interpolate the alpha value from startColor to targetColor
            float t = currentFadeTime / timeToFade;
            Color newColor = Color.Lerp(startColor, targetColor, t);

            spriteRenderer.color = newColor;

            yield return null;
        }

        Destroy(gameObject);    
    }
}
