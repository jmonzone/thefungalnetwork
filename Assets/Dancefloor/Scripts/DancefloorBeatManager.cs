using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancefloorBeatManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private DancefloorBeatUI glyphPrefab;
    [SerializeField] private RectTransform glyphParent;
    [SerializeField] private List<GlyphCountUI> glyphCountViews;

    [Header("Layout Settings")]
    [SerializeField] private float horizontalPadding = 50f;
    [SerializeField] private float verticalPadding = 80f;
    [SerializeField] private float spawnScale = 0.5f;

    [Header("Pattern Settings")]
    [SerializeField] private int maxSteps = 8;
    [SerializeField] private float waveFrequency = 2f;
    [SerializeField] private float wavePhase = 0f;

    private int patternStep = 0;
    private Coroutine beatRoutine;

    private float CanvasWidth => glyphParent.rect.width / glyphParent.transform.localScale.x;

    // -------- PUBLIC API --------
    public void StartBeats()
    {
        if (beatRoutine == null)
            beatRoutine = StartCoroutine(BeatLoop());
    }

    public void StopBeats()
    {
        if (beatRoutine != null)
        {
            StopCoroutine(beatRoutine);
            beatRoutine = null;
        }
    }

    // -------- INTERNAL COROUTINE --------
    private IEnumerator BeatLoop()
    {
        while (true)
        {
            float bpm = djReference.BPM > 0 ? djReference.BPM : 120f;
            float secondsPerStep = 60f / bpm / (maxSteps / 4f); // assumes 4 beats per measure

            HandleBeat(patternStep);
            Step();

            yield return new WaitForSeconds(secondsPerStep);
        }
    }

    // -------- BEAT LOGIC --------
    private void HandleBeat(int step)
    {
        // normalize left/right weighting
        float leftWeight = djReference.LeftValue;
        float rightWeight = djReference.RightValue;
        float total = leftWeight + rightWeight;
        leftWeight = (total > 0f) ? leftWeight / total : 0.5f;

        int leftSteps = Mathf.RoundToInt(leftWeight * maxSteps);
        List<int> leftStepIndices = new List<int>();
        if (leftSteps > 0)
        {
            float spacing = (float)maxSteps / leftSteps;
            for (int i = 0; i < leftSteps; i++)
                leftStepIndices.Add(Mathf.RoundToInt(i * spacing));
        }

        // pick track
        DJTrack chosenTrack;
        float chosenValue;
        if (leftStepIndices.Contains(patternStep))
        {
            chosenTrack = djReference.LeftTrack;
            chosenValue = djReference.LeftValue;
        }
        else
        {
            chosenTrack = djReference.RightTrack;
            chosenValue = djReference.RightValue;
        }

        if (chosenValue <= 0f)
        {
            glyphCountViews[chosenTrack == djReference.LeftTrack ? 0 : 1].gameObject.SetActive(false);
            return;
        }

        // update glyph count UI
        glyphCountViews[0].gameObject.SetActive(djReference.LeftValue > 0f);
        glyphCountViews[1].gameObject.SetActive(djReference.RightValue > 0f);
        glyphCountViews[0].SetGlyphCount(djReference.LeftTrack.Glyph);
        glyphCountViews[1].SetGlyphCount(djReference.RightTrack.Glyph);

        // spawn glyph
        SpawnGlyph(chosenTrack);
    }

    // -------- STEP MANAGEMENT --------
    private void Step()
    {
        patternStep = (patternStep + 1) % maxSteps;
    }

    // -------- GLYPH SPAWNING --------
    private void SpawnGlyph(DJTrack track)
    {
        float t = (float)patternStep / maxSteps;
        float wave = Mathf.Sin((t * Mathf.PI * 2f * waveFrequency) + wavePhase);
        float normalized = (wave + 1f) * 0.5f;

        float usableWidth = CanvasWidth - (horizontalPadding * 2f);
        float xPos = Mathf.Lerp(-usableWidth / 2f, usableWidth / 2f, normalized);
        Vector3 spawnPos = new Vector3(xPos, glyphParent.rect.yMax - verticalPadding, 0);
        Vector3 targetPos = new Vector3(xPos, glyphParent.rect.yMin + verticalPadding, 0);

        var glyph = Instantiate(glyphPrefab, glyphParent);
        glyph.transform.localPosition = spawnPos;
        glyph.transform.localScale = Vector3.one * spawnScale;

        StartCoroutine(glyph.FallRoutine(track.Glyph, spawnPos, targetPos));
    }
}
