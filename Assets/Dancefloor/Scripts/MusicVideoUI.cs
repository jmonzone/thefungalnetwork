using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicVideoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DancefloorReference musicVideoReference;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private MusicVideoGlyphUI glyphPrefab;
    [SerializeField] private RectTransform glyphParent; // Canvas container
    [SerializeField] private List<GlyphCountUI> glyphCountViews;
    [SerializeField] private Button exitButton;

    [Header("DDR Settings")]
    [SerializeField] private int columnCount = 4;
    [SerializeField] private float fallDuration = 3f; // slower = easier to collect
    [SerializeField] private float horizontalPadding = 50f; // pixels left/right padding
    [SerializeField] private float verticalPadding = 80f;   // pixels top/bottom padding
    [SerializeField] private float spawnScale = 0.5f;
    [SerializeField] private float endScale = 1f;
    [SerializeField] private float leafAmplitude = 20f; // side-to-side sway
    [SerializeField] private float rotationAmplitude = 15f; // degrees

    private float canvasWidth;

    private void Start()
    {
        exitButton.onClick.AddListener(musicVideoReference.ExitDancefloor);
    }

    private void OnEnable()
    {
        musicVideoReference.OnDancefloorStart += MusicVideoReference_OnMusicVideoStart;
        musicVideoReference.OnDancefloorExit += MusicVideoReference_OnMusicVideoEnd;
    }

    private void OnDisable()
    {
        musicVideoReference.OnDancefloorStart -= MusicVideoReference_OnMusicVideoStart;
        musicVideoReference.OnDancefloorExit -= MusicVideoReference_OnMusicVideoEnd;
    }

    private void MusicVideoReference_OnMusicVideoEnd()
    {
        djReference.OnBeat -= DjReference_OnBeat;
    }

    private void MusicVideoReference_OnMusicVideoStart()
    {
        djReference.OnBeat += DjReference_OnBeat;
    }

    private int patternStep = 0; // current step in the 8-step pattern
    private int maxSteps = 8;
    private int measureCount = 0; // used to alternate patterns

    // Define some patterns as column sequences (4 columns: 0-3)
    private readonly int[][] patterns = new int[][]
    {
    new int[] { 0, 1, 2, 3, 0, 1, 2, 3 }, // simple left to right
    new int[] { 0, 2, 1, 3, 0, 2, 1, 3 }, // zigzag
    new int[] { 1, 0, 3, 2, 1, 0, 3, 2 }, // reversed zigzag
    new int[] { 0, 3, 0, 3, 0, 3, 0, 3 }, // alternating edges
    };

    private void DjReference_OnBeat(int step)
    {
        canvasWidth = glyphParent.rect.width / glyphParent.transform.localScale.x;

        // normalize left/right
        float leftWeight = djReference.LeftValue;
        float rightWeight = djReference.RightValue;
        float total = leftWeight + rightWeight;

        if (total > 0f)
        {
            leftWeight /= total;
        }
        else
        {
            leftWeight = 0.5f;
        }

        // calculate number of steps per track
        int leftSteps = Mathf.RoundToInt(leftWeight * maxSteps);

        // create evenly spaced step indices for left track
        List<int> leftStepIndices = new List<int>();
        if (leftSteps > 0)
        {
            float spacing = (float)maxSteps / leftSteps;
            for (int i = 0; i < leftSteps; i++)
                leftStepIndices.Add(Mathf.RoundToInt(i * spacing));
        }

        // determine chosen track based on patternStep
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

        // skip if track has no value
        if (chosenValue <= 0f)
        {
            glyphCountViews[chosenTrack == djReference.LeftTrack ? 0 : 1].gameObject.SetActive(false);
            patternStep = (patternStep + 1) % maxSteps;
            return;
        }

        // enable/disable glyph count views
        glyphCountViews[0].gameObject.SetActive(djReference.LeftValue > 0f);
        glyphCountViews[1].gameObject.SetActive(djReference.RightValue > 0f);

        if (djReference.LeftValue == 0 || djReference.RightValue == 0)
        {
            glyphCountViews[0].SetGlyphCount(chosenTrack.Glyph);
        }
        else
        {
            glyphCountViews[0].SetGlyphCount(djReference.LeftTrack.Glyph);
            glyphCountViews[1].SetGlyphCount(djReference.RightTrack.Glyph);

        }


        // pick current pattern, alternate every 2 measures
        int patternIndex = (measureCount / 2) % patterns.Length;
        int[] currentPattern = patterns[patternIndex];

        // determine column from pattern
        int columnIndex = currentPattern[patternStep % maxSteps];

        // calculate positions
        float usableWidth = canvasWidth - (horizontalPadding * 2f);
        float columnSpacing = usableWidth / columnCount;
        float xPos = -usableWidth / 2f + (columnIndex + 0.5f) * columnSpacing;
        Vector3 spawnPos = new Vector3(xPos, glyphParent.rect.yMax - verticalPadding, 0);
        Vector3 targetPos = new Vector3(xPos, glyphParent.rect.yMin + verticalPadding, 0);

        // spawn glyph
        var glyph = Instantiate(glyphPrefab, glyphParent);
        glyph.transform.localPosition = spawnPos;
        glyph.transform.localScale = Vector3.one * spawnScale;

        // animate down
        StartCoroutine(glyph.FallRoutine(chosenTrack.Glyph, spawnPos, targetPos));

        // update glyph count view
        glyphCountViews[chosenTrack == djReference.LeftTrack ? 0 : 1].SetGlyphCount(chosenTrack.Glyph);

        // increment pattern step
        patternStep = (patternStep + 1) % maxSteps;
        if (patternStep == 0) measureCount++;
    }



}
