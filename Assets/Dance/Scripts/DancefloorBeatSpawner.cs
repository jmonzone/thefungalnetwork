using UnityEngine;

public class DancefloorBeatSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DancefloorBeatUI glyphPrefab;
    [SerializeField] private RectTransform glyphParent;

    [Header("Wave Settings")]
    [SerializeField] private float waveFrequency = 1f; // cycles per MaxSteps
    [SerializeField] private float wavePhase = 0f;     // radians offset
    [SerializeField] private int maxSteps = 8;

    [Header("Layout Settings")]
    [SerializeField] private float horizontalPadding = 50f;
    [SerializeField] private float verticalPadding = 80f;
    [SerializeField] private float spawnScale = 0.5f;

    private float CanvasWidth => glyphParent.rect.width / glyphParent.transform.localScale.x;
    public int MaxSteps => maxSteps;

    /// <summary>
    /// Spawns a glyph positioned along a sine wave.
    /// stepPosition can be an integer (step index) or fractional (between steps).
    /// </summary>
    public void SpawnGlyph(DJTrack track, float stepPosition)
    {
        float usableWidth = CanvasWidth - (horizontalPadding * 2f);

        // normalize step position into [0..1] range
        float t = stepPosition / maxSteps;

        // sine wave: -1..1 → map to usableWidth
        float wave = Mathf.Sin((t * Mathf.PI * 2f * waveFrequency) + wavePhase);
        float xPos = wave * (usableWidth * 0.5f);

        // Y positions (top spawn to bottom target)
        Vector3 spawnPos = new Vector3(xPos, glyphParent.rect.yMax - verticalPadding, 0);
        Vector3 targetPos = new Vector3(xPos, glyphParent.rect.yMin + verticalPadding, 0);

        var glyph = Instantiate(glyphPrefab, glyphParent);
        glyph.transform.localPosition = spawnPos;
        glyph.transform.localScale = Vector3.one * spawnScale;

        StartCoroutine(glyph.FallRoutine(track.Glyph, spawnPos, targetPos));
    }
}
