using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PhotoInteractionButton : MonoBehaviour
{
    [SerializeField] private FadeCanvasGroup fadeCanvasGroup;

    private Camera mainCamera;
    private Canvas canvas;

    private Vector3 targetPosition;
    private RectTransform rectTransform;

    private void Awake()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public IEnumerator Show(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        yield return fadeCanvasGroup.FadeIn();
    }

    private void Update()
    {
        Vector3 worldPos = targetPosition;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        // If using Screen Space - Overlay, you can just assign screenPos:
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rectTransform.position = screenPos;
        }
        else
        {
            // For Camera or World Space canvases, convert properly
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null,
                out localPos
            );
            rectTransform.localPosition = localPos;
        }
    }
}
