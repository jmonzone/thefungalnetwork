using System.Collections;
using UnityEngine;

public class MaterialFlasher : MonoBehaviour
{
    private Material[] originalMaterials;
    private Material[] childMaterials;
    private Renderer[] renderers;

    [SerializeField] private float flashDuration = 0.5f;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];
        childMaterials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
            childMaterials[i] = new Material(originalMaterials[i]);
            renderers[i].material = childMaterials[i];
        }
    }

    public void FlashWhite()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        Color targetColor = Color.white;
        float elapsedTime = 0f;

        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = elapsedTime / flashDuration;

            for (int i = 0; i < childMaterials.Length; i++)
            {
                childMaterials[i].color = Color.Lerp(originalMaterials[i].color, targetColor, lerpFactor);
            }

            yield return null;
        }

        for (int i = 0; i < childMaterials.Length; i++)
        {
            childMaterials[i].color = originalMaterials[i].color;
        }
    }
}
