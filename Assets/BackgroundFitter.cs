using UnityEngine;
using UnityEngine.UI;

public class BackgroundFitter : MonoBehaviour
{
    public enum HorizontalAlign { Left, Center, Right }

    [SerializeField] private HorizontalAlign horizontalAlign = HorizontalAlign.Center;

    private RectTransform rectTransform;
    private Image image;
    private RawImage rawImage;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        rawImage = GetComponent<RawImage>();
    }

    void LateUpdate()
    {
        Texture tex = null;

        if (image && image.sprite != null)
            tex = image.sprite.texture;
        else if (rawImage && rawImage.texture != null)
            tex = rawImage.texture;

        if (tex == null || rectTransform.parent == null) return;

        var parentRect = rectTransform.parent as RectTransform;
        float parentWidth = parentRect.rect.width;
        float parentHeight = parentRect.rect.height;

        float spriteRatio = (float)tex.width / tex.height;
        float parentRatio = parentWidth / parentHeight;

        Vector2 newSize;
        if (spriteRatio > parentRatio)
        {
            // Wider → match height, overflow width
            newSize = new Vector2(parentHeight * spriteRatio, parentHeight);
        }
        else
        {
            // Taller → match width, overflow height
            newSize = new Vector2(parentWidth, parentWidth / spriteRatio);
        }

        rectTransform.sizeDelta = newSize;

        // --- Alignment ---
        Vector2 offset = Vector2.zero;

        float extraWidth = newSize.x - parentWidth;
        if (extraWidth > 0)
        {
            switch (horizontalAlign)
            {
                case HorizontalAlign.Left:
                    offset.x = -extraWidth / 2f;
                    break;
                case HorizontalAlign.Center:
                    offset.x = 0f;
                    break;
                case HorizontalAlign.Right:
                    offset.x = extraWidth / 2f;
                    break;
            }
        }

        rectTransform.anchoredPosition = offset;
    }
}
