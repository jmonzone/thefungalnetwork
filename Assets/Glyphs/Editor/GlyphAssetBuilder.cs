using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public static class GlyphAssetBuilder
{
    public static void CreateGlyphAssets(string spritesFolder, string outputFolder)
    {
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        // Clear existing GlyphData assets in the output folder
        string[] existingGuids = AssetDatabase.FindAssets("t:GlyphData", new[] { outputFolder });
        foreach (string guid in existingGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AssetDatabase.DeleteAsset(path);
        }

        // Find all texture assets (not only sprites)
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { spritesFolder });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Ensure texture is a Sprite
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.SaveAndReimport();
            }

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null) continue;

            // Parse element counts from name
            if (!TryParseNameFromString(sprite.name, out var counts)) continue;

            // Create GlyphData asset
            string assetPath = Path.Combine(outputFolder, sprite.name + ".asset");
            GlyphData asset = ScriptableObject.CreateInstance<GlyphData>();
            asset.name = sprite.name;

            // Set private fields using reflection
            asset.GetType().GetField("fire", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(asset, counts.f);
            asset.GetType().GetField("water", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(asset, counts.w);
            asset.GetType().GetField("air", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(asset, counts.a);
            asset.GetType().GetField("earth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(asset, counts.e);
            asset.GetType().GetField("sprite", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(asset, sprite);

            asset.Initialize();

            AssetDatabase.CreateAsset(asset, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Glyph assets created.");
    }

    private static bool TryParseNameFromString(string name, out (int f, int w, int a, int e) counts)
    {
        counts = (0, 0, 0, 0);
        var matches = Regex.Matches(name.ToLower(), @"(\d+)([a-z])");
        if (matches.Count == 0) return false;

        foreach (Match m in matches)
        {
            int value = int.Parse(m.Groups[1].Value);
            char c = m.Groups[2].Value[0];
            switch (c)
            {
                case 'f': counts.f = value; break;
                case 'w': counts.w = value; break;
                case 'a': counts.a = value; break;
                case 'e': counts.e = value; break;
            }
        }

        return true;
    }

    [MenuItem("Tools/Create Glyph Assets")]
    public static void CreateAssets()
    {
        GlyphAssetBuilder.CreateGlyphAssets("Assets/Glyphs/Sprites", "Assets/Glyphs/Data");
    }
}