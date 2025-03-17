using UnityEngine;
using UnityEngine.UI;

public class ScoreSegment : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image segmentImage;
    [SerializeField] private Image segmentIcon;

    private float targetWidth;
    private float lerpSpeed = 10f; // Adjust for smoothness (higher = faster)

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        segmentImage = GetComponent<Image>();
    }

    public void SetFungal(NetworkFungal fungal)
    {
        Debug.Log($"SetFungal fungal " + !!fungal);
        Debug.Log($"SetFungal fungal.Data " + !!fungal.Data);
        Debug.Log($"SetFungal segmentImage " + !!segmentImage);
        Debug.Log($"SetFungal segmentIcon " + !!segmentIcon);

        segmentImage.color = fungal.Data.ActionColor;
        segmentIcon.sprite = fungal.Data.ActionImage;
        segmentIcon.enabled = true;
    }

    public void SetTargetWidth(float width)
    {
        targetWidth = width;
    }

    private void Update()
    {
        float currentWidth = rectTransform.sizeDelta.x;
        float newWidth = Mathf.Lerp(currentWidth, targetWidth, Time.deltaTime * lerpSpeed);

        rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
    }
}
