using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GlyphCollection : ScriptableObject
{
    [SerializeField] private List<GlyphData> glyphs;
    [SerializeField] private List<GlyphFusion> fusions;

    public List<GlyphData> Glyphs => glyphs;

    public bool TryFuse(GlyphData a, GlyphData b, out GlyphData fused)
    {
        foreach (var f in fusions)
        {
            if (f.Matches(a, b))
            {
                fused = f.Result;
                return true;
            }
        }
        fused = null;
        return false;
    }
}
