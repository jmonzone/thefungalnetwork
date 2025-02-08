using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameClock gameClock;
    [SerializeField] private Controller controller;
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private FadeCanvasGroup fadeCanvasGroup;
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI text;

    private Vignette vignette;

    public float pulseIntensity = 0.5f; // Peak intensity of the vignette during the pulse
    public float pulseDuration = 1f; // Duration of each pulse
    public int pulseCount = 3; // Number of pulses

    private void Awake()
    {
        gameClock.OnCountdown += GameClock_OnCountdown;
    }

    private void GameClock_OnCountdown(float arg0)
    {
        if ((arg0 / 60f) > 0.05f)
        {
            var header = "Another day, another bog";
            SetInformation(header, "Collect the mushrooms in order to escape");
            gameClock.OnCountdown -= GameClock_OnCountdown;
        }
    }

    private void OnEnable()
    {
        arena.OnMinionAssigned += Arena_OnMinionAssigned;
    }

    private void OnDisable()
    {
        arena.OnMinionAssigned -= Arena_OnMinionAssigned;
    }

    private void Arena_OnMinionAssigned()
    {
        var header = "Hate to say it, but...";
        SetInformation(header, "You have been possessed!");

        //if (controller.Volume.profile.TryGet<Vignette>(out var v))
        //{
        //    vignette = v;
        //    StartCoroutine(PulseVignette());
        //}
    }

    private void SetInformation(string header, string text)
    {
        this.header.text = header;
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
