using System.Collections;
using UnityEngine;

public class RaycastGunLine : MonoBehaviour
{
    
    LineRenderer gunLineRenderer;

    Vector2 lineStartPos;
    Vector2 lineEndPos;

    public void Initialize(Vector2 lineStartPos, Vector2 lineEndPos)
    {
        this.lineStartPos = lineStartPos;
        this.lineEndPos = lineEndPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        gunLineRenderer = GetComponent<LineRenderer>();

        Vector3[] linePositions = new Vector3[2]
        { lineStartPos, lineEndPos };

        gunLineRenderer.positionCount = linePositions.Length;
        gunLineRenderer.SetPositions(linePositions);

        StartCoroutine(IncreaseWidth());
    }

    IEnumerator IncreaseWidth()
    {
        for (int i = 0; i < 5; i++)
        {
            gunLineRenderer.startWidth += .01f;
            gunLineRenderer.endWidth += .025f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return StartCoroutine(DecreaseWidth());
    }

    IEnumerator DecreaseWidth()
    {
        for (int i = 0; i < 5; i++)
        {
            gunLineRenderer.startWidth -= .01f;
            gunLineRenderer.endWidth -= .025f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
