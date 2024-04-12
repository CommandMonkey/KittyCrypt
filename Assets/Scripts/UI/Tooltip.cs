using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    TextMeshProUGUI tooltip;
    [SerializeField] string[] possibleTooltips;
    [SerializeField] float showEachTooltipFor = 2f;
    string decidedTooltip;
    
    // Start is called before the first frame update
    void Start()
    {
        tooltip = GetComponent<TextMeshProUGUI>();
        StartCoroutine(SetTooltipText());
    }

    IEnumerator SetTooltipText()
    {
        decidedTooltip = possibleTooltips[Random.Range(0, possibleTooltips.Length)];
        tooltip.text = decidedTooltip;
        yield return new WaitForSeconds(showEachTooltipFor);
        StartCoroutine(SetTooltipText());
    }
}
