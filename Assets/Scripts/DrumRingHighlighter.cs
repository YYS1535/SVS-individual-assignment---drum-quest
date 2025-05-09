using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumRingHighlighter : MonoBehaviour
{
    public Material ringMaterial;
    public Color highlightColor = Color.white;
    public float intensity = 2f;
    public float duration = 0.3f;

    private Color originalEmission;

    void Start()
    {
        if (ringMaterial == null)
            ringMaterial = GetComponent<Renderer>().material;

        originalEmission = ringMaterial.GetColor("_EmissionColor");
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        ringMaterial.EnableKeyword("_EMISSION");
        ringMaterial.SetColor("_EmissionColor", highlightColor * intensity);

        yield return new WaitForSeconds(duration);

        ringMaterial.SetColor("_EmissionColor", originalEmission);
    }
}
