using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FadeCanvasGroup fadeCanvasGroup;
    [SerializeField] private TextMeshProUGUI text;

    private Vignette vignette;


    public float pulseIntensity = 0.5f; // Peak intensity of the vignette during the pulse
    public float pulseDuration = 1f; // Duration of each pulse
    public int pulseCount = 3; // Number of pulses

    private void OnEnable()
    {
        arena.OnIntroComplete += Arena_OnIntroComplete;
        arena.OnMinionAssigned += Arena_OnMinionAssigned;
    }

    private void Arena_OnMinionAssigned()
    {
        SetInformation("You have been possessed!");

        if (controller.Volume.profile.TryGet<Vignette>(out var v))
        {
            vignette = v;
            StartCoroutine(PulseVignette());
        }
    }

    private void Arena_OnIntroComplete()
    {
        SetInformation("Collect the mushrooms in order to escape");
    }

    private void SetInformation(string text)
    {
        this.text.text = text;

        StartCoroutine(Arena_OnIntroCompleteCoroutine());

    }

    private IEnumerator Arena_OnIntroCompleteCoroutine()
    {
        yield return fadeCanvasGroup.FadeIn();
        yield return new WaitForSeconds(2f);
        yield return fadeCanvasGroup.FadeOut();
    }


    private IEnumerator PulseVignette()
    {
        if (vignette == null) yield break;

        float initialIntensity = vignette.intensity.value;
        for (int i = 0; i < pulseCount; i++)
        {
            // Pulse up
            yield return PulseToIntensity(pulseIntensity, pulseDuration / 2);

            // Pulse down
            yield return PulseToIntensity(initialIntensity, pulseDuration / 2);
        }
    }

    private IEnumerator PulseToIntensity(float targetIntensity, float duration)
    {
        float startIntensity = vignette.intensity.value;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, elapsed / duration);
            yield return null;
        }

        vignette.intensity.value = targetIntensity;
    }
}
