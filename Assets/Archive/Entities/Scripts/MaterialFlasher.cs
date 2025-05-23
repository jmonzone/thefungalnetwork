﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialFlasher : MonoBehaviour
{
    public float flashDuration = 2f;

    private Material[] originalMaterials;
    private Material[] childMaterials;
    private Renderer[] renderers;
    private Coroutine coroutine;

    private Color originalColor; // Assuming all child materials share the same original emission color


    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        // Use lists to store only valid materials
        List<Material> validOriginalMaterials = new List<Material>();
        List<Material> validChildMaterials = new List<Material>();

        foreach (var renderer in renderers)
        {
            var material = renderer.material;

            // Skip materials that don't use the SoftSurfaceGraph shader
            if (!material.shader.name.Contains("SoftSurfaceGraph"))
                continue;

            // Add the valid materials to the lists
            validOriginalMaterials.Add(material);
            validChildMaterials.Add(new Material(material));

            // Set the new material to the renderer
            renderer.material = validChildMaterials[validChildMaterials.Count - 1];
        }

        // Convert the lists back to arrays if you need them as arrays elsewhere
        originalMaterials = validOriginalMaterials.ToArray();
        childMaterials = validChildMaterials.ToArray();
        originalColor = childMaterials[0].GetColor("_Color"); // Assuming all child materials share the same original emission color
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            FlashColor(Color.white);
        }
    }

    public void FlashColor(Color targetColor)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(FlashCoroutine(targetColor));
    }

    private IEnumerator FlashCoroutine(Color targetColor)
    {
        // Set the target emission intensity values
        float maxEmission = 10f;  // Max intensity for the flash (glowing)
        float minEmission = 0.5f;  // Min intensity (no glow)

        // Set target color for the flash effect

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
