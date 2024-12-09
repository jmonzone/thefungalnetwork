using System.Collections;
using UnityEngine;

public class MaterialFlasher : MonoBehaviour
{
    private Material[] originalMaterials;
    private Material[] childMaterials;
    private Renderer[] renderers;

    [SerializeField] private float flashDuration = 2f;
    [SerializeField] private Color targetColor;
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

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            FlashWhite();
            Debug.Log("hello");
        }
    }

    public void FlashWhite()
    {

        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        // Set the target emission intensity values
        float maxEmission = 10f;  // Max intensity for the flash (glowing)
        float minEmission = 0.5f;  // Min intensity (no glow)

        // Set target color for the flash effect
        Color originalColor = childMaterials[0].GetColor("_Color"); // Assuming all child materials share the same original emission color

        // Flash duration control (time to go from min to max and back to min)
        float halfDuration = flashDuration / 2f;  // Half of the duration for smooth in/out

        // Phase 1: Ease In (glow) + color transition
        float elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;

            // Ease-in effect using SmoothStep for emission
            float lerpFactor = Mathf.SmoothStep(minEmission, maxEmission, elapsedTime / halfDuration);
            // Lerp the color smoothly from the original color to the target color
            Color lerpedColor = Color.Lerp(originalColor, targetColor, elapsedTime / halfDuration);

            // Update emission and color on all materials
            for (int i = 0; i < childMaterials.Length; i++)
            {
                childMaterials[i].SetFloat("_Emission", lerpFactor);  // Update the emission intensity
                childMaterials[i].SetColor("_Color", lerpedColor);  // Update the color of the emission
            }

            yield return null;
        }

        // Phase 2: Ease Out (unglow) + revert color transition
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;

            // Ease-out effect using SmoothStep for emission
            float lerpFactor = Mathf.SmoothStep(maxEmission, minEmission, elapsedTime / halfDuration);
            // Lerp the color smoothly back to the original color
            Color lerpedColor = Color.Lerp(targetColor, originalColor, elapsedTime / halfDuration);

            // Update emission and color on all materials
            for (int i = 0; i < childMaterials.Length; i++)
            {
                childMaterials[i].SetFloat("_Emission", lerpFactor);  // Update the emission intensity
                childMaterials[i].SetColor("_Color", lerpedColor);  // Update the color of the emission
            }

            yield return null;
        }

        // Optionally, reset the emission intensity and color to original values
        for (int i = 0; i < childMaterials.Length; i++)
        {
            childMaterials[i].SetFloat("_Emission", originalMaterials[i].GetFloat("_Emission"));
            childMaterials[i].SetColor("_Color", originalColor);
        }
    }



}
