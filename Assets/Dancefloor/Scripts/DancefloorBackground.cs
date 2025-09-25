using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DancefloorBackground : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DancefloorReference dancefloorReference;
    [SerializeField] private ZoneController zoneController;
    [SerializeField] private DJTableReference dJTableReference;
    [SerializeField] private DancefloorBackgroundOverlay overlayEffects;
    [SerializeField] private FadeCanvasGroup fadeCanvasGroup;

    [Header("UI Crossfade")]
    [SerializeField] private Camera camA;
    [SerializeField] private Camera camB;
    [SerializeField] private Camera dominantCamera;
    [SerializeField] private RectTransform rect;
    [SerializeField] private RawImage rawImageA;
    [SerializeField] private RawImage rawImageB;
    [SerializeField] private Color baseColorA = Color.white;
    [SerializeField] private Color baseColorB = Color.white;
    [SerializeField] private float crossfadeDuration = 2f;

    private int[] stepsPerFadeOptions = new int[] { 4, 8, 16 }; // e.g., half bar, full bar
    private bool showingA = true;

    public Camera DominantCamera => dominantCamera;
    public RectTransform Rect => rect;

    [Header("Dancefloor Orbit")]
    [SerializeField] private float orbitRadius = 5f;
    [SerializeField] private float orbitHeight = 2f;
    [SerializeField] private float bobAmplitude = 0.5f;
    [SerializeField] private float orbitSpeedMultiplier = 1f;
    private float orbitAngleA;
    private float orbitAngleB;

    private int currentStepCounter = 0;
    private int currentStepsPerFade; // the current fade length in steps
    private bool waitingForFade = false;

    private void Start()
    {
        PickNextStepsPerFade();
        dominantCamera = camA;
    }

    private void OnEnable()
    {
        dancefloorReference.OnDancefloorStart += MusicVideoReference_OnMusicVideoStart;
        dancefloorReference.OnDancefloorExit += MusicVideoReference_OnMusicVideoEnd;
    }

    private void OnDisable()
    {
        dancefloorReference.OnDancefloorEnter -= MusicVideoReference_OnMusicVideoStart;
        dancefloorReference.OnDancefloorExit -= MusicVideoReference_OnMusicVideoEnd;
    }

    private void MusicVideoReference_OnMusicVideoStart()
    {
        rawImageA.color = baseColorA;
        rawImageB.color = new Color(baseColorB.r, baseColorB.g, baseColorB.b, 0f);

        dJTableReference.OnBeat += OnBeatStep;

        StartCoroutine(OrbitRoutine());
        StartCoroutine(fadeCanvasGroup.FadeIn(1));
    }

    private void MusicVideoReference_OnMusicVideoEnd()
    {
        dJTableReference.OnBeat -= OnBeatStep;

        StopAllCoroutines();
        StartCoroutine(fadeCanvasGroup.FadeOut());
    }

    private void PickNextStepsPerFade()
    {
        currentStepsPerFade = stepsPerFadeOptions[Random.Range(0, stepsPerFadeOptions.Length)];
    }

    private void OnBeatStep(int step)
    {
        if (waitingForFade) return; // fade already in progress

        currentStepCounter = (currentStepCounter + 1) % 8; // max 8 steps

        // Only trigger fade when step counter reaches the chosen number of steps
        if (currentStepCounter % currentStepsPerFade != 0)
            return;

        float beatDuration = dJTableReference.BeatDuration;
        float fadeDuration = beatDuration * crossfadeDuration;

        StartCoroutine(CrossfadeWithNextPick(fadeDuration));
    }

    private IEnumerator CrossfadeWithNextPick(float duration)
    {
        waitingForFade = true;
        yield return StartCoroutine(Crossfade(duration)); // your existing crossfade routine
        waitingForFade = false;

        // Pick next random stepsPerFade for the following fade
        PickNextStepsPerFade();

        overlayEffects.PickNextPattern();
    }

    private IEnumerator Crossfade(float duration)
    {
        float t = 0f;
        float startAlphaA = showingA ? 1f : 0f;
        float startAlphaB = showingA ? 0f : 1f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / duration);

            SetRawImageAlpha(Mathf.Lerp(startAlphaA, 1f - startAlphaA, blend),
                             Mathf.Lerp(startAlphaB, 1f - startAlphaB, blend));
            yield return null;
        }

        showingA = !showingA;
    }

    private IEnumerator OrbitRoutine()
    {
        float beatDuration = dJTableReference.BeatDuration;

        while (true)
        {
            float deltaAngle = 360f * (Time.deltaTime / (beatDuration * 4f)) * orbitSpeedMultiplier;

            orbitAngleA += deltaAngle;
            orbitAngleB -= deltaAngle;

            Vector3 center = zoneController.transform.position;

            float radA = Mathf.Deg2Rad * orbitAngleA;
            float radB = Mathf.Deg2Rad * orbitAngleB;

            float bobA = Mathf.Sin(Time.time * (2f * Mathf.PI / beatDuration) * orbitSpeedMultiplier) * bobAmplitude;
            float bobB = Mathf.Sin(Time.time * (2f * Mathf.PI / beatDuration) * orbitSpeedMultiplier + Mathf.PI) * bobAmplitude;

            camA.transform.position = center + new Vector3(Mathf.Cos(radA), orbitHeight + bobA, Mathf.Sin(radA)) * orbitRadius;
            camA.transform.LookAt(center + Vector3.up * 1f);

            camB.transform.position = center + new Vector3(Mathf.Cos(radB), orbitHeight + bobB, Mathf.Sin(radB)) * orbitRadius;
            camB.transform.LookAt(center + Vector3.up * 1f);

            yield return null;
        }
    }

    private void SetRawImageAlpha(float alphaA, float alphaB)
    {
        if (alphaA > alphaB) dominantCamera = camA;
        else dominantCamera = camB;

        rawImageA.color = new Color(baseColorA.r, baseColorA.g, baseColorA.b, alphaA);
        rawImageB.color = new Color(baseColorB.r, baseColorB.g, baseColorB.b, alphaB);
    }
}
