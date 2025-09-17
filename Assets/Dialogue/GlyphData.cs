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
    [SerializeField] private Sprite sprite;

    public string Id => id;
    public Sprite Sprite => sprite;
}
