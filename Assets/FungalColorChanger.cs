using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FungalColorChanger : UnitBehaviour
{
    [System.Serializable]
    public struct ColumnConfig
    {
        public bool overrideColor;   // if true, override with palette
        [Range(0, 7)] public int paletteIndex; // which palette color to use
    }

    public enum FungalPalette
    {
        Original = 1,
        Red = 2,
        Yellow = 3,
        Blue = 4,
        Green = 5
    }

    [SerializeField] private FungalPalette selectedPalette = FungalPalette.Original;
    [SerializeField] private Texture2D originalCache; // assign prefab's texture

    [SerializeField] private Color[] paletteColors = new Color[8];   // the 8 base colors
    [SerializeField] private ColumnConfig[] columnConfigs = new ColumnConfig[8]; // one per column

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Cache the original texture for this Fungal
        Renderer rend = Unit.GetComponentInChildren<Renderer>();
        if (rend.material.mainTexture is Texture2D tex)
        {
            Debug.Log("Unit " + Unit.name);
            originalCache = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
            originalCache.SetPixels32(tex.GetPixels32());
            originalCache.Apply();
        }
    }

    protected override void OnBehaviourStart()
    {
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyUp(KeyCode.Space))
        {
            foreach (var renderer in Unit.GetComponentsInChildren<Renderer>())
            {
                ApplySelectedPalette(renderer);
            }
        }
    }

    private void ApplySelectedPalette(Renderer renderer)
    {
        if (originalCache == null) return;

        Texture2D tex = new Texture2D(originalCache.width, originalCache.height, TextureFormat.RGBA32, false);
        tex.SetPixels32(originalCache.GetPixels32());

        int blockSize = 2;
        int blocksY = tex.height / blockSize; // 4 / 2 = 2

        for (int by = 0; by < blocksY; by++)
        {
            for (int bx = 0; bx < 8; bx++)
            {
                int px = bx * blockSize;
                int py = by * blockSize;

                // Original color
                Color origColor = tex.GetPixel(px, py);

                // Get mapping for this column
                int mapIndex = Unit.Instance.Data.ColumnMapping[bx];

                // -1 = keep original
                Color newColor = origColor;
                if (mapIndex >= 0 && mapIndex < paletteColors.Length)
                {
                    newColor = paletteColors[mapIndex];
                }

                // Apply to 2x2 block
                for (int y = 0; y < blockSize; y++)
                {
                    for (int x = 0; x < blockSize; x++)
                    {
                        tex.SetPixel(px + x, py + y, newColor);
                    }
                }
            }
        }

        tex.Apply();

        // Assign new material instance to avoid shared material issues
        renderer.material = new Material(renderer.material);
        renderer.material.mainTexture = tex;
    }
}
