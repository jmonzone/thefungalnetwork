using UnityEngine;
using System.Collections;
using TMPro;

public class CollectionPopupUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText;

    public CanvasGroup canvasGroup;        // For fading
    public RectTransform rectTransform;    // For movement
    public float floatDistance = 50f;      // Vertical move in pixels
    public float duration = 1f;            // Total popup lifetime

    private Vector3 worldPosition;          // The world position to anchor to
    private bool isPlaying = false;

    private Canvas canvas;
    private Camera mainCamera;

    public void Initialize(Canvas canvas, Camera camera)
    {
        this.canvas = canvas;
        this.mainCamera = camera;
    }

    /// <summary>
    /// Starts the popup animation anchored to a world position.
    /// </summary>
    public void PlayPopup(Vector3 worldPos, int count)
    {
        countText.text = $"+{count}";

        mainCamera = Camera.main;
        worldPosition = worldPos;
        isPlaying = true;

        canvasGroup.alpha = 1f;
        gameObject.SetActive(true);

        StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        float elapsed = 0f;

        // Calculate initial screen and canvas-local positions
        Vector2 startAnchoredPos = WorldToCanvasLocalPosition(worldPosition);
        Vector2 endOffset = Vector2.up * floatDistance;

        while (elapsed < duration)
        {
            if (!isPlaying) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Update anchored position relative to world
            Vector2 anchoredPos = WorldToCanvasLocalPosition(worldPosition);
            rectTransform.anchoredPosition = Vector2.Lerp(anchoredPos, anchoredPos + endOffset, t);

            // Fade out after halfway
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp01((t - 0.5f) * 2));

            yield return null;
        }

        isPlaying = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Converts a world position to a local position relative to the canvas.
    /// </summary>
    private Vector2 WorldToCanvasLocalPosition(Vector3 worldPos)
    {
        Vector2 screenPoint = mainCamera.WorldToScreenPoint(worldPos);
        RectTransform canvasRect = canvas.transform as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out Vector2 localPoint);

        return localPoint;
    }
}
