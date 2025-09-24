using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GlyphUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform glyphPalette;
    [SerializeField] private GlyphDropZone glyphDropZone;
    [SerializeField] private GlyphController glyphPrefab;
    [SerializeField] private GlyphCollection glyphCollection;
    [SerializeField] private GlyphEmitterUI glyphEmitterUI;
    [SerializeField] private DialogueReference dialogue;
    [SerializeField] private InventoryReference inventoryReference;
    [SerializeField] private List<GlyphPalleteSlotUI> glyphSlots;

    [Header("Spiral Animation Settings")]
    [SerializeField] private float separateDuration = 0.2f;   // time to separate outward before spiraling
    [SerializeField] private float spiralDuration = 0.8f;     // time to spiral inward
    [SerializeField] private float spiralRadius = 75f;        // radius of spiral path
    [SerializeField] private int spiralRotations = 1;         // number of rotations inward
    [SerializeField] private float phaseOffsetA = 0f;         // starting angle offset for glyph A
    [SerializeField] private float phaseOffsetB = Mathf.PI;   // opposite side for glyph B

    [Header("Fusion Animation Settings")]
    [SerializeField] private float elasticDuration = 1.2f;
    [SerializeField] private float elasticOvershoot = 1.4f;
    [SerializeField] private float elasticPower = 10f;
    [SerializeField] private float elasticFrequency = 13f;
    [SerializeField] private float glowIntensity = 1f;

    [Header("Fusion Fail Settings")]
    [SerializeField] private float failSeparateDistance = 80f;
    [SerializeField] private float failSeparateDuration = 0.25f;
    [SerializeField] private float failClashDistance = 30f;
    [SerializeField] private float failClashDuration = 0.2f;
    [SerializeField] private float failSettleDuration = 0.35f;
    [SerializeField] private float failScaleUpAmount = 0.2f;
    [SerializeField] private float failWobbleStrength = 5f;
    [SerializeField] private Color failSettleTint = new Color(1f, 0.9f, 0.9f); // subtle pinkish neutral

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fusion1Audio;
    [SerializeField] private AudioClip fusion2Audio;
    [SerializeField] private AudioClip failAudio;

    private List<GlyphController> glyphControllers = new List<GlyphController>();

    public event UnityAction OnGlyphDialogueComplete;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        glyphPalette.GetComponentsInChildren(true, glyphSlots);
        glyphDropZone.OnGlyphPlaced += OnPalleteGlyphDropped;
    }

    private void OnEnable()
    {
        inventoryReference.OnGlyphCountChanged += UpdateGlyphSlots;
    }

    private void OnDisable()
    {
        inventoryReference.OnGlyphCountChanged -= UpdateGlyphSlots;
    }

    private void UpdateGlyphSlots(GlyphData arg0, int arg1)
    {
        var i = 0;
        foreach (var glyph in inventoryReference.Glyphs.Keys)
        {
            glyphSlots[i].SetCount(inventoryReference.Glyphs[glyph]);
            i++;
        }
    }

    private GlyphController SpawnGlyph(GlyphData glyph, Vector3 position, bool isPaletteGlyph, List<string> words)
    {
        var glyphObj = Instantiate(glyphPrefab, isPaletteGlyph ? glyphPalette : glyphDropZone.transform);
        glyphObj.GetComponent<RectTransform>().position = position;
        glyphObj.Initialize(glyph, isPaletteGlyph, words );

        glyphObj.OnGlyphFused += OnGlyphsFused;

        glyphControllers.Add(glyphObj);
        return glyphObj;
    }

    public void StartGlyphDialogue()
    {
        // Filter glyphs
        var selectedGlyphs = glyphCollection.Glyphs
            .Where(glyph => glyph.Tier > 1 && glyph.Element.HasFlag(dialogue.Unit.Instance.Element))
            .OrderBy(g => Random.value)
            .Take(1)
            .ToList();

        // Example
        string dialogueText = dialogue.Dialogue.Text;

        // Split into words using spaces (and remove empty entries)
        List<string> words = dialogueText
            .Split(' ')
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .ToList();

        glyphEmitterUI.EmitGlyphs(selectedGlyphs, words, dialogue.Unit.transform.position);

        InitializePalleteGlyphs();
    }

    private void InitializePalleteGlyphs()
    {
        var i = 0;
        foreach (var glyph in inventoryReference.Glyphs.Keys)
        {
            glyphSlots[i].SetCount(inventoryReference.Glyphs[glyph]);
            SpawnGlyph(glyph, glyphSlots[i].Rect.position, isPaletteGlyph: true, new List<string>());
            i++;
        }
    }

    private void OnPalleteGlyphDropped(GlyphController placedGlyph, GlyphDropZone zone)
    {
        if (zone == glyphDropZone)
        {
            if (placedGlyph.IsPalleteGlyph)
            {
                inventoryReference.DecreaseShrune(placedGlyph.Glyph);
                if (inventoryReference.Glyphs[placedGlyph.Glyph] > 0) SpawnGlyph(placedGlyph.Glyph, placedGlyph.OriginalPosition, true, new List<string>());
            }
        }
        else
        {
            placedGlyph.ReturnToOriginalParent();
        }
    }

    private void OnGlyphsFused(GlyphController dragged, GlyphController target)
    {
        if (glyphCollection.TryFuse(dragged.Glyph, target.Glyph, out GlyphData fusedGlyph))
        {
            StartCoroutine(FuseRoutine(dragged, target, fusedGlyph));
        }
        else
        {
            StartCoroutine(FailRoutine(dragged, target));
        }
    }

    private IEnumerator FuseRoutine(GlyphController dragged, GlyphController target, GlyphData fusedGlyph)
    {
        ToggleInteractable(false);

        // Midpoint between dragged + target
        Vector3 clashPoint = (dragged.RectTransform.position + target.RectTransform.position) / 2f;

        // Each starts at clashpoint, separates outward, then spirals inward
        Coroutine animA = StartCoroutine(
            dragged.AnimateSeparateAndSpiralIn(clashPoint, separateDuration, spiralDuration, spiralRadius, spiralRotations, phaseOffsetA)
        );
        Coroutine animB = StartCoroutine(
            target.AnimateSeparateAndSpiralIn(clashPoint, separateDuration, spiralDuration, spiralRadius, spiralRotations, phaseOffsetB)
        );

        audioSource.clip = fusion1Audio;
        audioSource.Play();

        yield return animA;
        yield return animB;

        // Cleanup old glyphs
        glyphControllers.Remove(dragged);
        Destroy(dragged.gameObject);
        glyphControllers.Remove(target);
        Destroy(target.gameObject);

        // Spawn fused glyph
        var fused = SpawnGlyph(fusedGlyph, clashPoint, false, new List<string>());

        audioSource.clip = fusion2Audio;
        audioSource.Play();

        // Elastic scale-up with glow using manager parameters
        yield return StartCoroutine(fused.AnimateElasticSpawn(
            elasticDuration, elasticOvershoot, elasticPower, elasticFrequency, glowIntensity));

        var matchingGlyph = glyphEmitterUI.GlyphControllers.Find(controller => controller.Glyph == fused.Glyph);

        if (matchingGlyph)
        {
            StartCoroutine(MatchRoutine(fused, matchingGlyph));
        }

        ToggleInteractable(true);
    }

    private IEnumerator MatchRoutine(GlyphController fusedGlyph, BlockingGlyphController matchingGlyph)
    {
        yield return fusedGlyph.MoveAndSlot(matchingGlyph.RectTransform);
        yield return matchingGlyph.GlowAndRelease();

        yield return new WaitForSeconds(3f);

        yield return matchingGlyph.CleanupTextControllers();

        OnGlyphDialogueComplete?.Invoke();
    }

    private IEnumerator FailRoutine(GlyphController dragged, GlyphController target)
    {
        ToggleInteractable(false);

        Vector2 clashPoint = (dragged.RectTransform.anchoredPosition + target.RectTransform.anchoredPosition) / 2f;

        // Trigger fail animations on both glyphs, mirrored directions
        Vector2 dirA = (dragged.RectTransform.anchoredPosition - clashPoint).normalized;
        Vector2 dirB = (target.RectTransform.anchoredPosition - clashPoint).normalized;

        audioSource.clip = failAudio;
        audioSource.Play();

        Coroutine animA = StartCoroutine(dragged.AnimateFusionFail(
            dirA,
            failSeparateDistance,
            failSeparateDuration,
            failClashDistance,
            failClashDuration,
            failSettleDuration,
            failScaleUpAmount,
            failWobbleStrength,
            failSettleTint
        ));

        Coroutine animB = StartCoroutine(target.AnimateFusionFail(
            dirB,
            failSeparateDistance,
            failSeparateDuration,
            failClashDistance,
            failClashDuration,
            failSettleDuration,
            failScaleUpAmount,
            failWobbleStrength,
            failSettleTint
        ));

        yield return animA;
        yield return animB;

        ToggleInteractable(true);
    }

    public void ToggleInteractable(bool value)
    {
        foreach (var glyph in glyphControllers)
        {
            glyph.ToggleInteractable(value);
        }
    }

    public void HideGlyphUI()
    {
        foreach(var glyph in glyphControllers)
        {
            if (!glyph.IsPalleteGlyph)
            {
                Destroy(glyph.gameObject);
            }
        }

        glyphControllers = new List<GlyphController>();
    }
}
