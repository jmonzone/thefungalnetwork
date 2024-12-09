using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crocodile : MonoBehaviour
{
    [SerializeField] private GameObject overheadUI;
    [SerializeField] private Gradient hitColor;
    [SerializeField] private float hitColorDuration = 2f;
    [SerializeField] private float flashDuration = 0.5f; // Time for flash to complete
    [SerializeField] private float maxHealth = 3f;
    private Slider slider;

    private Gradient defaultColor;
    private ParticleSystem particles;
    private Coroutine colorChangeCoroutine;


    private Material[] originalMaterials;
    private Material[] childMaterials;
    private Renderer[] renderers;

    private Animator animator;

    private void Awake()
    {
        overheadUI.SetActive(true);

        slider = GetComponentInChildren<Slider>(includeInactive: true);
        slider.maxValue = maxHealth;
        slider.minValue = 0;
        slider.value = maxHealth;

        particles = GetComponentInChildren<ParticleSystem>();
        defaultColor = particles.colorOverLifetime.color.gradient;

        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // Get all child renderers and store their materials
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];
        childMaterials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            // Store the original materials of the children
            originalMaterials[i] = renderers[i].material;
            // Create a new instance of the material to modify it during the flash
            childMaterials[i] = new Material(originalMaterials[i]);
            renderers[i].material = childMaterials[i];
        }
    }

    private IEnumerator FlashWhite()
    {
        // Flash the materials to white
        Color targetColor = Color.white;
        float elapsedTime = 0f;

        // Lerp to white quickly
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = elapsedTime / flashDuration;

            // Lerp through each child's material
            for (int i = 0; i < childMaterials.Length; i++)
            {
                childMaterials[i].color = Color.Lerp(originalMaterials[i].color, targetColor, lerpFactor);
            }

            yield return null;
        }

        // Restore the original materials after flash duration
        for (int i = 0; i < childMaterials.Length; i++)
        {
            childMaterials[i].color = originalMaterials[i].color;
        }
    }

    public void Damage()
    {
        slider.value--;

        if (slider.value == 0)
        {
            animator.Play("Death");
        }

        // Stop any ongoing color transition to avoid conflicts
        if (colorChangeCoroutine != null)
        {
            StopCoroutine(colorChangeCoroutine);
        }

        // Start the transition to hitColor
        colorChangeCoroutine = StartCoroutine(LerpGradient(hitColor, hitColorDuration, () =>
        {
            // After hitColor, transition back to defaultColor
            colorChangeCoroutine = StartCoroutine(LerpGradient(defaultColor, hitColorDuration));
        }));

        StartCoroutine(FlashWhite());
    }

    private IEnumerator LerpGradient(Gradient targetGradient, float duration, System.Action onComplete = null)
    {
        var colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient currentGradient = colorOverLifetime.color.gradient;
        Gradient lerpedGradient = new Gradient();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float progress = t / duration;

            // Lerp between the current and target gradients
            GradientColorKey[] colorKeys = LerpColorKeys(currentGradient.colorKeys, targetGradient.colorKeys, progress);
            GradientAlphaKey[] alphaKeys = LerpAlphaKeys(currentGradient.alphaKeys, targetGradient.alphaKeys, progress);

            lerpedGradient.SetKeys(colorKeys, alphaKeys);

            // Apply the interpolated gradient
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(lerpedGradient);

            yield return null;
        }

        // Ensure the final gradient is fully applied
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(targetGradient);

        // Call the onComplete callback if provided
        onComplete?.Invoke();
    }

    private GradientColorKey[] LerpColorKeys(GradientColorKey[] fromKeys, GradientColorKey[] toKeys, float t)
    {
        int keyCount = Mathf.Min(fromKeys.Length, toKeys.Length);
        GradientColorKey[] resultKeys = new GradientColorKey[keyCount];

        for (int i = 0; i < keyCount; i++)
        {
            resultKeys[i] = new GradientColorKey(
                Color.Lerp(fromKeys[i].color, toKeys[i].color, t),
                Mathf.Lerp(fromKeys[i].time, toKeys[i].time, t)
            );
        }

        return resultKeys;
    }

    private GradientAlphaKey[] LerpAlphaKeys(GradientAlphaKey[] fromKeys, GradientAlphaKey[] toKeys, float t)
    {
        int keyCount = Mathf.Min(fromKeys.Length, toKeys.Length);
        GradientAlphaKey[] resultKeys = new GradientAlphaKey[keyCount];

        for (int i = 0; i < keyCount; i++)
        {
            resultKeys[i] = new GradientAlphaKey(
                Mathf.Lerp(fromKeys[i].alpha, toKeys[i].alpha, t),
                Mathf.Lerp(fromKeys[i].time, toKeys[i].time, t)
            );
        }

        return resultKeys;
    }
}
