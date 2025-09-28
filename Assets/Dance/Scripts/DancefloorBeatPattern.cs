using System;
using System.Collections.Generic;
using UnityEngine;

public class DancefloorBeatPattern : MonoBehaviour
{
    public enum PatternType
    {
        Simple,
        ZigZag,
        Reversed,
        AlternatingEdges,
        Wave
    }

    public enum PatternMode
    {
        Specific,   // always use selectedPattern
        Alternate   // cycle through patterns by measureCount
    }

    [Header("Pattern Settings")]
    [SerializeField] private PatternMode mode = PatternMode.Specific;
    [SerializeField] private PatternType selectedPattern = PatternType.Simple;

    [Header("Wave Settings")]
    [SerializeField] private float waveFrequency = 2f;
    [SerializeField] private float wavePhase = 0f;

    private int maxSteps = 8;

    private readonly Dictionary<PatternType, int[]> patterns =
        new Dictionary<PatternType, int[]>
    {
        { PatternType.Simple,           new int[] { 0, 1, 2, 3, 0, 1, 2, 3 } },
        { PatternType.ZigZag,           new int[] { 0, 2, 1, 3, 0, 2, 1, 3 } },
        { PatternType.Reversed,         new int[] { 1, 0, 3, 2, 1, 0, 3, 2 } },
        { PatternType.AlternatingEdges, new int[] { 0, 3, 0, 3, 0, 3, 0, 3 } }
    };

    public int GetColumnIndex(int measureCount, int patternStep, int columnCount)
    {
        PatternType type = selectedPattern;

        if (mode == PatternMode.Alternate)
        {
            // cycle through available pattern types based on measureCount
            int enumCount = Enum.GetValues(typeof(PatternType)).Length;
            type = (PatternType)((measureCount / 2) % enumCount);
        }

        if (type == PatternType.Wave)
        {
            float t = (float)patternStep / maxSteps;
            float wave = Mathf.Sin((t * Mathf.PI * 2f * waveFrequency) + wavePhase);
            float normalized = (wave + 1f) * 0.5f;
            return Mathf.FloorToInt(normalized * (columnCount - 1));
        }
        else
        {
            int[] currentPattern = patterns[type];
            return currentPattern[patternStep % maxSteps];
        }
    }

    public int MaxSteps => maxSteps;
}
