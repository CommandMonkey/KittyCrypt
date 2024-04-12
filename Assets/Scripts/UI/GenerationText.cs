using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class GenerationText : MonoBehaviour
{
    TextMeshProUGUI generatingTextMesh;
    [SerializeField] string generatingText;
    [SerializeField] float timeBetweenMoreDots = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        generatingTextMesh = GetComponent<TextMeshProUGUI>();
        StartCoroutine(LoadingTextDungeon());
    }

    IEnumerator LoadingTextDungeon()
    {
        generatingTextMesh.text = generatingText;
        yield return new WaitForSeconds(timeBetweenMoreDots);
        generatingTextMesh.text = generatingText + ".";
        yield return new WaitForSeconds(timeBetweenMoreDots);
        generatingTextMesh.text = generatingText + "..";
        yield return new WaitForSeconds(timeBetweenMoreDots);
        generatingTextMesh.text = generatingText + "...";
        yield return new WaitForSeconds(timeBetweenMoreDots);
        StartCoroutine(LoadingTextDungeon());
    }
}
