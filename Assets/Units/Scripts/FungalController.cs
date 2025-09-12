using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public interface IJob
{
    public bool IsAble { get; }
    public bool IsMoving { get; }
    public Vector3 TargetPosition { get; }
    public event UnityAction OnIsAbleChanged;
    public event UnityAction OnIsMovingChanged;
}

public enum FungalState
{
    WANDER,
    DIALOGUE,
    FOLLOW
}

public class FungalController : UnitController
{
    [Header("Fungal References")]
    [SerializeField] private GameObject chatIcon;
    [SerializeField] private DialogueReference dialogueReference;

    public bool IsAtDestination => unitDestination.IsAtDestination;

    private UnitDestination unitDestination;
    private UnitFollow unitFollow;
    private Animator animator;

    protected override void Awake()
    {
        base.Awake();

        unitDestination = GetComponent<UnitDestination>();
        unitFollow = GetComponent<UnitFollow>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        dialogueReference.OnIsActiveChanged += DialogueReference_OnIsActiveChanged;
    }

    private void DialogueReference_OnIsActiveChanged()
    {
        OnProximityChanged(false);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        dialogueReference.OnIsActiveChanged -= DialogueReference_OnIsActiveChanged;
    }

    public override void Initialize(UnitInstance instance)
    {
        Quaternion randomYRotation = Quaternion.Euler(0, UnityEngine.Random.Range(135f, 225f), 0);
        renderRoot = Instantiate(instance.Data.Prefab, Vector3.zero, randomYRotation, transform).transform;
        animator = GetComponentInChildren<Animator>();
        base.Initialize(instance);

        // Cache the original texture for this Fungal
        Renderer rend = renderRoot.GetComponentInChildren<Renderer>();
        if (rend.material.mainTexture is Texture2D tex)
        {
            originalCache = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
            originalCache.SetPixels32(tex.GetPixels32());
            originalCache.Apply();
        }
    }

    [ContextMenu("Apply Palette")]
    private void TestPalette()
    {
        foreach (var renderer in renderRoot.GetComponentsInChildren<Renderer>())
        {
            ApplySelectedPalette(renderer);
        }
    }


    protected override void Update()
    {
        base.Update();
 
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
    [SerializeField] private Color redBase = Color.red;
    [SerializeField] private Color yellowBase = Color.yellow;
    [SerializeField] private Color blueBase = Color.blue;
    [SerializeField] private Color greenBase = Color.green;

    [Header("Palette Settings")]
    [SerializeField, Range(0f, 1f)] private float hueBlend = 1f;       // overall hue blend factor
    [SerializeField, Range(0f, 1f)] private float satVariation = 0.05f;
    [SerializeField, Range(0f, 1f)] private float valVariation = 0.05f;
    [SerializeField, Range(0f, 1f)] private float columnStep = 0.25f;  // how much each column moves toward base

    private void ApplySelectedPalette(Renderer renderer)
    {
        if (originalCache == null) return;

        Texture2D tex = new Texture2D(originalCache.width, originalCache.height, TextureFormat.RGBA32, false);
        tex.SetPixels32(originalCache.GetPixels32());

        Color baseColor = selectedPalette switch
        {
            FungalPalette.Red => redBase,
            FungalPalette.Yellow => yellowBase,
            FungalPalette.Blue => blueBase,
            FungalPalette.Green => greenBase,
            _ => Color.clear
        };

        if (baseColor == Color.clear) return;

        int blockSize = 2;
        int blocksX = tex.width / blockSize;  // 16 / 2 = 8
        int blocksY = tex.height / blockSize; // 4 / 2 = 2

        // Left 4 columns gradient: blend each column toward base color
        for (int by = 0; by < blocksY; by++)
        {
            for (int bx = 0; bx < 4; bx++)
            {
                // Column 0 = step toward original, column 3 = fully base color
                float blend = Mathf.Clamp01(columnStep * bx);

                int px = bx * blockSize;
                int py = by * blockSize;

                Color origColor = tex.GetPixel(px, py);

                Color newColor = BlendHSV(origColor, baseColor, blend);

                // Apply to 2x2 pixels
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

    // HSV vector-style blend that works even for pastel or brown colors
    private Color BlendHSV(Color orig, Color baseCol, float blend)
    {
        // Convert to HSV
        Color.RGBToHSV(orig, out float h1, out float s1, out float v1);
        Color.RGBToHSV(baseCol, out float h2, out float s2, out float v2);

        // Compute shortest delta for hue
        float dh = Mathf.DeltaAngle(h1 * 360f, h2 * 360f) / 360f;
        float newH = (h1 + dh * blend * hueBlend) % 1f;

        // Interpolate saturation/value toward base
        float newS = Mathf.Clamp01(s1 + (s2 - s1) * blend * (1f - satVariation) + satVariation);
        float newV = Mathf.Clamp01(v1 + (v2 - v1) * blend * (1f - valVariation) + valVariation);

        return Color.HSVToRGB(newH, newS, newV);
    }




    public void SetDestination(Vector3 destination)
    {
        unitDestination.SetDestination(destination);
        ApplyBehaviour(unitDestination);
    }

    public void SetTarget(Transform target)
    {
        unitFollow.SetTarget(target);
        ApplyBehaviour(unitFollow);
    }

    public void TriggerDeath()
    {
        animator.Play("Death");
    }

    public void TriggerRespawn()
    {
        animator.SetTrigger("Respawn");
    }

    public override void OnProximityChanged(bool value)
    {
        base.OnProximityChanged(value);
        chatIcon.SetActive(!dialogueReference.IsActive && value);
    }
}
