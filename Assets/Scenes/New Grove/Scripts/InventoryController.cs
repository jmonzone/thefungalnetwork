using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface ICollectable
{
    public Transform Transform { get; }
    public event UnityAction OnCollect;
}

public class InventoryController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mushroomCountText;
    [SerializeField] private Slider insightSlider;
    [SerializeField] private Image[] rainbowImages; // Assign the 2 images in inspector
    private Color[] rainbowImagesOriginalColors;

    public Color highlightColorA = Color.cyan;  // Dimmer or base pulse
    public Color highlightColorB = Color.white; // Brighter or target pulse
    public float pulseSpeed = 2f;
    public float transitionDuration = 0.3f;
    public float sliderLerpSpeed = 5f; // Adjust this for faster/slower lerp

    private Coroutine sliderLerpRoutine;
    private Coroutine pulseRoutine;
    private Coroutine transitionRoutine;

    public int MushroomCount { get; private set; }
    public event UnityAction<ICollectable> OnCollect;
    public event UnityAction<int> OnMushroomCountChanged;

    private void Awake()
    {
        var mushrooms = FindObjectsOfType<InteractableMushroom>(true);
        foreach(var mushroom in mushrooms)
        {
            mushroom.OnCollect += () =>
            {
                SetMushroomCount(MushroomCount + 1);                
                OnCollect?.Invoke(mushroom);
            };
        }

        insightSlider.minValue = 0;
        insightSlider.maxValue = 5;
        insightSlider.value = 0;

        rainbowImagesOriginalColors = new Color[rainbowImages.Length];
        for (var i = 0; i < rainbowImages.Length; i++)
        {
            rainbowImagesOriginalColors[i] = rainbowImages[i].color;
        }
    }


    public void SetMushroomCount(int value)
    {
        MushroomCount = value;

        // Start the lerp coroutine
        if (sliderLerpRoutine != null) StopCoroutine(sliderLerpRoutine);

        sliderLerpRoutine = StartCoroutine(LerpSliderValue(insightSlider.value, MushroomCount));

        mushroomCountText.text = MushroomCount.ToString();
        OnMushroomCountChanged?.Invoke(MushroomCount);

        if (MushroomCount >= insightSlider.maxValue)
        {
            if (pulseRoutine == null)
            {
                if (transitionRoutine != null)
                    StopCoroutine(transitionRoutine);

                transitionRoutine = StartCoroutine(TransitionToHighlightThenPulse());
            }
        }
        else
        {
            if (pulseRoutine != null)
            {
                StopCoroutine(pulseRoutine);
                pulseRoutine = null;
            }

            if (transitionRoutine != null)
                StopCoroutine(transitionRoutine);

            transitionRoutine = StartCoroutine(TransitionToNormalColor());
        }

    }

    private IEnumerator LerpSliderValue(float startValue, float endValue)
    {
        float t = 0f;
        while (Mathf.Abs(insightSlider.value - endValue) > 0.01f)
        {
            t += Time.deltaTime * sliderLerpSpeed;
            insightSlider.value = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }

        insightSlider.value = endValue; // Snap exactly at end
    }

    private IEnumerator TransitionToHighlightThenPulse()
    {
        // Lerp to highlightColorA
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / transitionDuration;

            foreach (var img in rainbowImages)
            {
                Color lerped = Color.Lerp(img.color, highlightColorA, t);
                img.color = lerped;
            }

            yield return null;
        }

        // Start pulsing once reached
        pulseRoutine = StartCoroutine(PulseColorSineWave());
    }

    private IEnumerator TransitionToNormalColor()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / transitionDuration;

            var i = 0;
            foreach (var img in rainbowImages)
            {
                Color lerped = Color.Lerp(img.color, rainbowImagesOriginalColors[i], t);
                img.color = lerped;
                i++;
            }

            yield return null;
        }
    }


    private IEnumerator PulseColorSineWave()
    {
        while (true)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) + 1f) / 2f;
            Color pulsingColor = Color.Lerp(highlightColorA, highlightColorB, t);

            foreach (var img in rainbowImages)
                img.color = pulsingColor;

            yield return null;
        }
    }

}
