using System;
using UnityEngine;

[CreateAssetMenu]
public class GlyphData : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private int fire;
    [SerializeField] private int water;
    [SerializeField] private int air;
    [SerializeField] private int earth;

    [SerializeField] private Element element;
    [SerializeField] private int tier;

    public int Fire => fire;
    public int Water => water;
    public int Air => air;
    public int Earth => earth;

    public int Tier => tier;
    public Element Element => element;
    public Sprite Sprite => sprite;

    public void Initialize()
    {
        // Reset values
        fire = water = air = earth = 0;
        element = Element.NONE;
        tier = 0;

        // Parse counts from name (e.g. "3a2f1w")
        var matches = System.Text.RegularExpressions.Regex.Matches(name, @"(\d+)([a-zA-Z])");
        foreach (System.Text.RegularExpressions.Match m in matches)
        {
            int value = int.Parse(m.Groups[1].Value);
            char symbol = char.ToLower(m.Groups[2].Value[0]);

            switch (symbol)
            {
                case 'f': fire = value; break;
                case 'w': water = value; break;
                case 'a': air = value; break;
                case 'e': earth = value; break;
            }
        }

        // Compute tier as total number of components
        tier = fire + water + air + earth;

        // Compute weighted element

        var weightThreshold = .4;

        int total = tier;
        if (total > 0)
        {
            // Weight each element proportional to its count
            float fWeight = fire / (float)total;
            float wWeight = water / (float)total;
            float aWeight = air / (float)total;
            float eWeight = earth / (float)total;

            element = Element.NONE;

            // If weight > 0, include the element
            if (fWeight > weightThreshold) element |= Element.FIRE;
            if (wWeight > weightThreshold) element |= Element.WATER;
            if (aWeight > weightThreshold) element |= Element.AIR;
            if (eWeight > weightThreshold) element |= Element.EARTH;
        }
    }
}

[Flags]
public enum Element
{
    NONE = 0,
    FIRE = 1 << 0,
    WATER = 1 << 1,
    EARTH = 1 << 2,
    AIR = 1 << 3
}