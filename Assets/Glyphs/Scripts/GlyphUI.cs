using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GlyphUI : MonoBehaviour
{
    [SerializeField] private Transform glyphPalette;
    [SerializeField] private GlyphDropZone glyphDropZone;
    [SerializeField] private GlyphController glyphPrefab;
    [SerializeField] private GlyphCollection glyphCollection;

    [SerializeField] private RectTransform glyphSlot1;
    [SerializeField] private RectTransform glyphSlot2;
    [SerializeField] private RectTransform glyphSlot3;
    [SerializeField] private RectTransform glyphSlot4;

    [SerializeField] private Image glyphImage;
    [SerializeField] private TextMeshProUGUI fungalText;

    [SerializeField] private VertexGradient normalColor;
    [SerializeField] private VertexGradient blurColor;

    [Header("Initial Glyphs")]
    [SerializeField] private GlyphData glyph1;
    [SerializeField] private GlyphData glyph2;
    [SerializeField] private GlyphData glyph3;
    [SerializeField] private GlyphData glyph4;

    private GlyphData targetGlyph;

    private List<GlyphController> glyphControllers = new List<GlyphController>();

    public event UnityAction OnGlyphMatched;

    private void Awake()
    {
        SpawnGlyph(glyph1, glyphSlot1.anchoredPosition, isPaletteGlyph: true);
        SpawnGlyph(glyph2, glyphSlot2.anchoredPosition, isPaletteGlyph: true);
        SpawnGlyph(glyph3, glyphSlot3.anchoredPosition, isPaletteGlyph: true);
        SpawnGlyph(glyph4, glyphSlot4.anchoredPosition, isPaletteGlyph: true);

        glyphDropZone.OnGlyphPlaced += HandleGlyphPlaced;
    }

    private void HandleGlyphPlaced(GlyphController placedGlyph, GlyphDropZone zone)
    {
        if (zone == glyphDropZone)
        {
            if (placedGlyph.IsPalleteGlyph)
            {
                SpawnGlyph(placedGlyph.Glyph, placedGlyph.OriginalPosition, true);
            }
        }
        else
        {
            placedGlyph.ReturnToOriginalParent();
        }
    }

    private GlyphController SpawnGlyph(GlyphData glyph, Vector3 position, bool isPaletteGlyph)
    {
        var glyphObj = Instantiate(glyphPrefab, isPaletteGlyph ? glyphPalette : glyphDropZone.transform);
        glyphObj.GetComponent<RectTransform>().anchoredPosition = position;
        glyphObj.Initialize(glyph, isPaletteGlyph);

        glyphObj.OnGlyphFused += GlyphObj_OnGlyphFused;

        glyphControllers.Add(glyphObj);
        return glyphObj;
    }

    private void GlyphObj_OnGlyphFused(GlyphController dragged, GlyphController target)
    {
        if (glyphCollection.TryFuse(dragged.Glyph, target.Glyph, out GlyphData fusedGlyph))
        {
            glyphControllers.Remove(dragged);
            Destroy(dragged.gameObject);

            glyphControllers.Remove(target);
            Destroy(target.gameObject);

            var targetRect = target.GetComponent<RectTransform>();
            var fusedGlyphController = SpawnGlyph(fusedGlyph, targetRect.anchoredPosition, false);

            if (targetGlyph == fusedGlyph)
            {
                var fusedRect = fusedGlyphController.GetComponent<RectTransform>();
                StartCoroutine(MatchGlyphRoutine(fusedRect));
            }
        }
    }

    private IEnumerator MatchGlyphRoutine(RectTransform fusedRect)
    {
        var duration = 1.5f;
        var targetRect = glyphImage.GetComponent<RectTransform>();

        // store starting state
        Vector3 startPos = fusedRect.position;          // world position
        Vector3 targetPos = targetRect.position;        // world position
        Vector3 startScale = fusedRect.localScale;
        Vector3 targetScale = targetRect.localScale;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // smooth interpolation in world space
            Vector3 newWorldPos = Vector3.Lerp(startPos, targetPos, t);
            fusedRect.position = newWorldPos;

            fusedRect.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        // snap to final
        fusedRect.position = targetPos;
        fusedRect.localScale = targetScale;

        fusedRect.gameObject.SetActive(false);
        StartCoroutine(GlowEffect(glyphImage, Color.cyan, 1.5f));
    }

    private IEnumerator GlowEffect(Image img, Color glowColor, float duration = 0.5f)
    {
        Color startColor = img.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.PingPong(elapsed * 2f, 1f); // pulse in/out
            img.color = Color.Lerp(startColor, glowColor, t);
            yield return null;
        }

        img.color = startColor; // reset after glow

        glyphImage.enabled = false;
        //fungalText.colorGradient = normalColor;
        OnGlyphMatched?.Invoke();
    }


    public void StartGlyphDialogue(GlyphData glyph)
    {
        targetGlyph = glyph;

        fungalText.colorGradient = blurColor;

        glyphImage.sprite = glyph.Sprite;
        glyphImage.enabled = false;

        glyphPalette.gameObject.SetActive(true);
        glyphDropZone.gameObject.SetActive(true);
    }


    public void BlockDialogueWithGlyph()
    {
        glyphImage.enabled = true;
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

        glyphImage.enabled = false;
        glyphPalette.gameObject.SetActive(false);
        glyphDropZone.gameObject.SetActive(false);
    }
}
