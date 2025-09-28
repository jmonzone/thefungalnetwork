using System.Linq;
using UnityEngine;

public class UnitColorPalette : UnitBehaviour
{
    [SerializeField] private Texture2D originalCache; // assign prefab's texture
    [SerializeField] private ColorPalette colorPalette; // the 8 base colors
    [SerializeField] private Color primaryColor;

    public Color PrimaryColor => primaryColor;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Cache the original texture for this Fungal
        Renderer rend = Unit.GetComponentInChildren<Renderer>();
        if (rend.material.mainTexture is Texture2D tex)
        {
            originalCache = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
            originalCache.SetPixels32(tex.GetPixels32());
            originalCache.Apply();
        }

        SetColorPalette(Unit.Instance.ColorPalette);
    }

    protected override void OnBehaviourStart()
    {
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyUp(KeyCode.Space))
        {
            ApplyColorPalette();
        }
    }

    public void SetColorPalette(ColorPalette colorPalette)
    {
        this.colorPalette = colorPalette;
        ApplyColorPalette();
    }

    private void ApplyColorPalette()
    {
        foreach (var renderer in Unit.GetComponentsInChildren<Renderer>())
        {
            ApplySelectedPalette(renderer);
        }
    }

    private void ApplySelectedPalette(Renderer renderer)
    {
        if (!originalCache) return;

        Texture2D tex = new Texture2D(originalCache.width, originalCache.height, TextureFormat.RGBA32, false);
        tex.SetPixels32(originalCache.GetPixels32());

        int blockSize = 2;
        int blocksY = tex.height / blockSize; // 4 / 2 = 2

        if (colorPalette)
        {
            primaryColor = colorPalette.PrimaryColor;
        }
        else if (!colorPalette)
        {
            primaryColor = GetPrimaryColor(tex);
            return;
        }


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
                if (mapIndex >= 0 && mapIndex < 3)
                {
                    newColor = mapIndex switch
                    {
                        0 => colorPalette.PrimaryColor,
                        1 => colorPalette.SecondaryColor,
                        2 => colorPalette.AccentColor,
                        _ => colorPalette.PrimaryColor,
                    };
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

    private Color GetPrimaryColor(Texture2D tex, int blockSize = 2)
    {
        int blocksY = tex.height / blockSize;
        int blocksX = tex.width / blockSize;

        for (int by = 0; by < blocksY; by++)
        {
            for (int bx = 0; bx < blocksX; bx++)
            {
                int mapIndex = Unit.Instance.Data.ColumnMapping[bx];

                // Only care about PrimaryColor
                if (mapIndex == 0)
                {
                    int px = bx * blockSize;
                    int py = by * blockSize;

                    // Just grab the representative pixel (they're uniform in this block)
                    return tex.GetPixel(px, py);
                }
            }
        }

        Debug.LogWarning("No primary color found in texture!");
        return Color.white;
    }
}
