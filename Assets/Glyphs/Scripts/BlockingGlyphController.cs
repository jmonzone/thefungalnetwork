using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockingGlyphController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image glyphImage;
    [SerializeField] private GlyphTextController textPrefab;

    [Header("Settings")]
    [SerializeField] private Color glowColor = Color.cyan;
    [SerializeField] private float glowDuration = 1.5f;
    [SerializeField] private float wordSpacing = 10f;
    [SerializeField] float dissipateDuration = 0.8f;
    [SerializeField] private float cleanupDuration = 1.0f; // or get from the text controller

    [Header("Runtime")]
    [SerializeField] private GlyphData glyph;
    [SerializeField] private RectTransform container;
    [SerializeField] private List<GlyphTextController> textControllers = new List<GlyphTextController>();

    public GlyphData Glyph => glyph;
    public RectTransform RectTransform => rectTransform;

    public void Initialize(GlyphData glyph, List<string> words, RectTransform container)
    {
        this.glyph = glyph;
        this.container = container;

        glyphImage.sprite = glyph.Sprite;
        glyphImage.SetNativeSize();

        Vector2 center = Vector2.zero; // sigil center in local space
        float maxRotationOffset = 10f; // random wiggle around the leaning angle

        foreach (var word in words)
        {
            var textController = Instantiate(textPrefab, transform);
            textController.transform.SetAsFirstSibling();

            // Random offset from center
            Vector2 randomOffset = new Vector2(
                Random.Range(-25f, 25f),
                Random.Range(-12f, 12f)
            );

            textController.transform.localPosition = randomOffset * 2f;
            textController.transform.localScale = Vector3.one;

            // Compute angle toward center
            Vector2 toCenter = center - randomOffset;
            float angle = Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg;

            // Clamp to upright range so words aren't upside down
            if (angle > 90f) angle -= 180f;
            if (angle < -90f) angle += 180f;

            // Add small random wiggle around the leaning angle
            float randomTwist = Random.Range(-maxRotationOffset, maxRotationOffset);
            textController.transform.localRotation = Quaternion.Euler(0f, 0f, angle + randomTwist);

            textController.Initialize(word);
            textControllers.Add(textController);
        }
    }

    public IEnumerator Animate(Vector3 startCanvasPos, Vector3 targetCanvasPos, float duration, float scalePulse)
    {
        rectTransform.localPosition = startCanvasPos;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);

            rectTransform.localPosition = Vector3.Lerp(startCanvasPos, targetCanvasPos, Mathf.SmoothStep(0, 1, normalized));
            rectTransform.localScale = Vector3.one * (1f + scalePulse * Mathf.Sin(normalized * Mathf.PI));

            yield return null;
        }

        rectTransform.localPosition = targetCanvasPos;
        rectTransform.localScale = Vector3.one;
    }

    public IEnumerator GlowAndRelease()
    {
        Color startColor = glyphImage.color;
        float elapsed = 0f;

        // --- Glow pulse ---
        while (elapsed < glowDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.PingPong(elapsed * 2f, 1f);
            glyphImage.color = Color.Lerp(startColor, glowColor, t);
            yield return null;
        }

        // --- Release / Dissipate ---
        elapsed = 0f;
        Vector3 startScale = rectTransform.localScale;
        Vector3 targetScale = startScale * 1.5f;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsed < dissipateDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dissipateDuration;

            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            glyphImage.color = Color.Lerp(glowColor, targetColor, t);

            yield return null;
        }

        glyphImage.enabled = false;

        ReleaseWordsAsSentence();
    }

    public void ReleaseWordsAsSentence(float maxWidth = -1f)
    {
        if (textControllers.Count == 0) return;

        float containerWidth = maxWidth > 0f ? maxWidth : container.rect.width;

        float x = 0f;
        float y = 0f;
        float lineHeight = 0f;
        List<Vector3> targetPositions = new List<Vector3>();

        List<float> lineWidths = new List<float>();
        List<int> lineBreaks = new List<int>();
        float currentLineWidth = 0f;

        // First pass: compute positions and line widths
        for (int i = 0; i < textControllers.Count; i++)
        {
            var word = textControllers[i];
            float wordWidth = word.Text.preferredWidth * word.transform.localScale.x;
            float wordHeight = word.Text.preferredHeight * word.transform.localScale.y;

            if (lineHeight < wordHeight) lineHeight = wordHeight;

            // Wrap if exceeding container width
            if (x + wordWidth > containerWidth / 2f)
            {
                lineWidths.Add(currentLineWidth);
                lineBreaks.Add(i); // store start of new line
                x = 0f;
                y -= lineHeight + wordSpacing;
                currentLineWidth = 0f;
                lineHeight = wordHeight;
            }

            targetPositions.Add(new Vector3(x + wordWidth / 2f, y, 0f));
            x += wordWidth + wordSpacing;
            currentLineWidth += wordWidth + wordSpacing;
        }

        lineWidths.Add(currentLineWidth); // add last line width

        // Centering horizontally and vertically
        int lineIndex = 0;
        int wordIndexInLine = 0;

        for (int i = 0; i < textControllers.Count; i++)
        {
            // Determine line width for current line
            if (lineBreaks.Contains(i))
            {
                lineIndex++;
                wordIndexInLine = 0;
            }

            float lineWidth = lineWidths[lineIndex];

            Vector3 pos = targetPositions[i];

            // Shift to center horizontally
            pos.x -= lineWidth / 2f;

            // Shift to center vertically
            pos.y += lineHeight / 2f;

            textControllers[i].SetTargetPosition(pos);
            wordIndexInLine++;
        }
    }

    /// <summary>
    /// Tells all child text controllers to fade out and move away, then clears the list.
    /// </summary>
    public IEnumerator CleanupTextControllers()
    {
        if (textControllers.Count == 0) yield break;

        // Trigger each text controller's own fade/move behavior
        foreach (var text in textControllers)
        {
            if (text)
                StartCoroutine(text.ReleaseToEar());
        }

        yield return new WaitForSeconds(cleanupDuration);

        // Clear the references
        textControllers.Clear();
    }

}
