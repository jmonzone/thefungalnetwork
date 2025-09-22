using UnityEngine;
using System.Collections;

public class GlyphCollectController : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer glyphRenderer;       // Assign the world-space sprite renderer
    public GameObject pulseRingPrefab;         // Optional ring prefab
    public float spiralRadius = 50f;
    public float spiralHeight = 100f;
    public float spiralDuration = 0.8f;
    public float spiralRotations = 2f;
    public float scaleUp = 1.5f;
    public float scaleDuration = 0.5f;
    public float jumpHeight = 0.75f;

    [Header("Glyph Data")]
    public Sprite glyphSprite;

    public IEnumerator PlayReveal()
    {
        if (glyphRenderer == null)
        {
            Debug.LogWarning("GlyphCollectReveal: No SpriteRenderer assigned!");
            yield break;
        }

        // --- Setup ---
        glyphRenderer.gameObject.SetActive(true);
        glyphRenderer.sprite = glyphSprite;
        transform.localScale = Vector3.zero;
        glyphRenderer.color = Color.white;

        Vector3 playerPos = transform.localPosition; // assume attached to player
        float elapsed = 0f;

        // --- Spiral Animation with scale ---
        while (elapsed < spiralDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / spiralDuration);

            // Spiral around player
            float angle = t * spiralRotations * 2f * Mathf.PI;
            float radius = Mathf.Lerp(spiralRadius, 0f, t); // spiral inward
            float yOffset = Mathf.Lerp(0f, spiralHeight, t); // rise above player

            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            transform.localPosition = playerPos + offset + Vector3.up * yOffset;

            // Scale up as it rises
            float scaleT = Mathf.SmoothStep(0f, 1f, t);
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * scaleUp, scaleT);

            yield return null;
        }

        // --- Elastic scale-up pulse ---
        elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 pulseScale = Vector3.one * scaleUp;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;

            // Elastic ease out for scale-up
            float elastic = Mathf.Sin(-Mathf.PI / 2f * (t + 1f)) * Mathf.Pow(2f, -10f * t) + 1f;
            transform.localScale = Vector3.Lerp(startScale, pulseScale, elastic);

            yield return null;
        }

        // --- Jump into head animation ---
        elapsed = 0f;
        Vector3 headPosition = playerPos + Vector3.up * 1f; // target near player head
        Vector3 startPos = transform.localPosition;
        Vector3 endScale = Vector3.one * 0.3f; // shrink while moving

        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scaleDuration);

            // horizontal / linear interpolation
            Vector3 horizontal = Vector3.Lerp(startPos, headPosition, t);

            // vertical parabolic arc
            float verticalOffset = jumpHeight * 4f * t * (1f - t); // peaks at t=0.5
            transform.localPosition = horizontal + Vector3.up * verticalOffset;

            // smooth scale down
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            transform.localScale = Vector3.Lerp(startScale, endScale, smoothT);

            yield return null;
        }

        // --- Deactivate ---
        gameObject.SetActive(false);


    }
}
