using UnityEngine;
using TMPro;
using System.Collections;

public class GlyphTextController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private Wiggler wiggler;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;       // speed to straighten rotation
    [SerializeField] private float glowDuration = 1f;
    [SerializeField] private Color glowColor = Color.cyan;
    [SerializeField] private Color targetColor = Color.white;
    [SerializeField] private float maxFinalRotation = 10f;   // max small rotation for readability
    [SerializeField] private float wiggleDilute = 0.1f;
    [SerializeField] private float releaseDuration = 1f;

    public TMP_Text Text => text;

    private Vector3 targetLocalPos;
    private bool moving = false;
    private Quaternion targetRotation;

    public void Initialize(string word)
    {
        text.text = word;
    }

    public void SetTargetPosition(Vector3 localPos)
    {
        targetLocalPos = localPos;
        moving = true;

        // Random small rotation around z-axis for final readable orientation
        float finalAngle = Random.Range(-maxFinalRotation, maxFinalRotation);
        targetRotation = Quaternion.Euler(0f, 0f, finalAngle);

        StartCoroutine(GlowAndFadeIn());
        StartCoroutine(DiluteWiggle());
    }

    private void Update()
    {
        if (moving)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetLocalPos,
                Time.deltaTime * moveSpeed
            );

            // Lerp rotation from initial wiggle toward final readable rotation
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }

    private IEnumerator GlowAndFadeIn()
    {
        float elapsed = 0f;
        Color startColor = text.color;

        // Glow pulse
        while (elapsed < glowDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / glowDuration);

            text.color = Color.Lerp(startColor, glowColor, Mathf.PingPong(t * 2f, 1f));
            yield return null;
        }

        // Fade into final opaque color
        elapsed = 0f;
        float fadeDuration = 0.5f;
        startColor = text.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            text.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        text.color = targetColor; // ensure fully opaque
    }

    private IEnumerator DiluteWiggle(float duration = 1f)
    {
        if (wiggler == null) yield break;

        float elapsed = 0f;
        float startMaxOffset = wiggler.maxOffset;
        float startMaxRotation = wiggler.maxRotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            wiggler.maxOffset = Mathf.Lerp(startMaxOffset, startMaxOffset * wiggleDilute, t); // dilute to 20%
            wiggler.maxRotation = Mathf.Lerp(startMaxRotation, startMaxRotation * wiggleDilute, t);

            yield return null;
        }

        // finalize wiggle dilution
        wiggler.maxOffset = startMaxOffset * wiggleDilute;
        wiggler.maxRotation = startMaxRotation * wiggleDilute;
    }

    /// <summary>
    /// Handles its own fading and moving logic when released from a blocking glyph.
    /// Moves the word from one ear to another and fades it out.
    /// </summary>
    public IEnumerator ReleaseToEar()
    {
        Vector3 startPos = transform.localPosition;

        // Decide direction randomly or based on side
        Vector3 targetOffset = new Vector3(Random.Range(-200f, 200f), Random.Range(50f, 150f), 0f);
        Vector3 targetPos = startPos + targetOffset;

        Color startColor = text.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        float elapsed = 0f;

        while (elapsed < releaseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / releaseDuration);

            transform.localPosition = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
            text.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        text.color = targetColor;
        gameObject.SetActive(false); // optionally destroy instead
    }
}
