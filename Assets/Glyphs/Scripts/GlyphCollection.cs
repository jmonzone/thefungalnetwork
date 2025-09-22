using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class GlyphCollection : ScriptableObject
{
    [SerializeField] private List<GlyphData> glyphs;
    [SerializeField] private List<GlyphFusion> fusions = new List<GlyphFusion>();

    private Dictionary<(int f, int w, int a, int e), GlyphData> lookup = new Dictionary<(int f, int w, int a, int e), GlyphData>();

    public List<GlyphData> Glyphs => glyphs;
    public List<GlyphFusion> Fusions => fusions;

    public void Initialize()
    {
        BuildLookup();
        BuildAllFusions();
    }

    public void SetGlyphs(List<GlyphData> glyphs)
    {
        this.glyphs = glyphs.OrderBy(glyph => glyph.Tier).ToList();
    }

    private void BuildLookup()
    {
        lookup = new Dictionary<(int, int, int, int), GlyphData>();
        foreach (var g in glyphs)
        {
            if (g == null) continue;
            var key = (g.Fire, g.Water, g.Air, g.Earth);
            if (!lookup.ContainsKey(key))
                lookup[key] = g;
        }
    }

    /// <summary>
    /// Generates all possible fusions where two glyphs combine into an existing glyph.
    /// </summary>
    private void BuildAllFusions()
    {
        fusions.Clear();

        int count = glyphs.Count;
        for (int i = 0; i < count; i++)
        {
            for (int j = i; j < count; j++) // allow same glyph fusion
            {
                var a = glyphs[i];
                var b = glyphs[j];
                if (a == null || b == null) continue;

                var combinedCounts = (a.Fire + b.Fire, a.Water + b.Water, a.Air + b.Air, a.Earth + b.Earth);

                if (lookup.TryGetValue(combinedCounts, out var result))
                {
                    fusions.Add(new GlyphFusion(a, b, result));
                }
            }
        }

        Debug.Log($"Total fusions generated: {fusions.Count}");
    }

    /// <summary>
    /// Try to fuse two glyphs. Returns the resulting glyph if it exists.
    /// </summary>
    public bool TryFuse(GlyphData a, GlyphData b, out GlyphData fused)
    {
        Debug.Log($"fire {a.Fire}");
        var combinedCounts = (a.Fire + b.Fire, a.Water + b.Water, a.Air + b.Air, a.Earth + b.Earth);
        Debug.Log($"lookup {lookup != null}");
        Debug.Log($"lookup keys {lookup.Keys.Count}");

        return lookup.TryGetValue(combinedCounts, out fused);
    }
}

[Serializable]
public class GlyphFusion
{
    [SerializeField] private GlyphData glyphA;
    [SerializeField] private GlyphData glyphB;
    [SerializeField] private GlyphData result;

    public GlyphData GlyphA => glyphA;
    public GlyphData GlyphB => glyphB;
    public GlyphData Result => result;

    public GlyphFusion(GlyphData a, GlyphData b, GlyphData r)
    {
        glyphA = a;
        glyphB = b;
        result = r;
    }
}
