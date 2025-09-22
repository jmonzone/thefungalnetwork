#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

public class GlyphCollectionEditor
{
    [MenuItem("Tools/Assign Glyphs")]
    public static void AssignGlyphsToCollection()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is GlyphCollection collection)
            {
                var glyphs = new List<GlyphData>();
                string[] guids = AssetDatabase.FindAssets("t:GlyphData", new[] { "Assets/Glyphs" });

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    GlyphData asset = AssetDatabase.LoadAssetAtPath<GlyphData>(path);
                    if (asset != null && !glyphs.Contains(asset))
                        glyphs.Add(asset);
                }

                collection.SetGlyphs(glyphs);
                EditorUtility.SetDirty(collection);
            }
        }
    }
}
#endif
