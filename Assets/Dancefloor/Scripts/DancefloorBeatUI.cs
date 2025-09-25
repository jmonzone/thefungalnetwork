using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DancefloorBeatUI : MonoBehaviour
{
    [SerializeField] private InventoryReference inventory;

    [Header("References")]
    public RectTransform rect;
    public CanvasGroup canvasGroup;
    public Image glyphImage;

    [Header("Settings")]
    public float spawnScale = 0.5f;
    public float endScale = 1f;
    public float fallDuration = 3f;
    public float leafAmplitude = 20f;
    public float rotationAmplitude = 15f;
    public float spawnPopTime = 0.2f;
    public float endScaleDownTime = 0.2f;
    public float successScaleUp = 1.5f;
    public float successFadeTime = 0.3f;

    private float centerX;
    private bool collected = false;
    private GlyphData glyph;

    public IEnumerator FallRoutine(GlyphData glyph, Vector3 start, Vector3 target)
    {
        this.glyph = glyph;
        glyphImage.sprite = glyph.Sprite;

        float elapsed = 0f;
        rect.localPosition = start;
        rect.localScale = Vector3.zero;

        // --- Spawn pop ---
        while (elapsed < spawnPopTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / spawnPopTime);
            rect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * spawnScale * 1.3f, t);
            yield return null;
        }

        rect.localScale = Vector3.one * spawnScale;

        // Record centerX for sway
        centerX = rect.localPosition.x;
        elapsed = 0f;

        while (elapsed < fallDuration)
        {
            if (collected) yield break; // stop movement if collected

            elapsed += Time.deltaTime;
            float t = elapsed / fallDuration;
            float eased = t * t * (3f - 2f * t); // smoothstep

            Vector3 basePos = Vector3.Lerp(start, target, eased);
            float sway = Mathf.Sin(t * Mathf.PI * 2f) * leafAmplitude * (1f - eased);
            rect.localPosition = new Vector3(centerX + sway, basePos.y, basePos.z);
            rect.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(t * Mathf.PI * 2f) * rotationAmplitude);

            yield return null;
        }

        // --- Scale down at end ---
        elapsed = 0f;
        Vector3 startScale = rect.localScale;
        while (elapsed < endScaleDownTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / endScaleDownTime);
            rect.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Call this when the glyph is collected (e.g., by AuraController collision)
    /// </summary>
    public void Collect()
    {
        if (collected) return;
        collected = true;
        StopAllCoroutines();
        StartCoroutine(SuccessRoutine());

        if (glyph != null)
            inventory.IncreaseShrune(glyph);
    }

    private IEnumerator SuccessRoutine()
    {
        float elapsed = 0f;
        Vector3 startScale = rect.localScale;
        float startAlpha = canvasGroup != null ? canvasGroup.alpha : 1f;

        while (elapsed < successFadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / successFadeTime);

            rect.localScale = Vector3.Lerp(startScale, startScale * successScaleUp, t);
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}
