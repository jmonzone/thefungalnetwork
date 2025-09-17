using System;
using UnityEngine;

[Serializable]
public class GlyphFusion
{
    [SerializeField] private GlyphData glyphA;
    [SerializeField] private GlyphData glyphB;
    [SerializeField] private GlyphData result;

    public GlyphData GlyphA => glyphA;
    public GlyphData GlyphB => glyphB;
    public GlyphData Result => result;

    public bool Matches(GlyphData a, GlyphData b)
    {
        return (a == glyphA && b == glyphB) || (a == glyphB && b == glyphA);
    }
}

[CreateAssetMenu]
public class GlyphData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private int tier;
    [SerializeField] private Element element;
    [SerializeField] private Sprite sprite;

    public string Id => id;
    public int Tier => tier;
    public Element Element => element;
    public Sprite Sprite => sprite;
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