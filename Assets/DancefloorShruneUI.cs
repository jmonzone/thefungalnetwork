using UnityEngine;
using UnityEngine.UI;

public class DancefloorShruneUI : MonoBehaviour
{
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private Image shruneImage;       // main image (alpha-controlled)
    [SerializeField] private Image shruneFillImage;   // optional fill bar

    public void UpdateUI(float value, float min, float max)
    {
        if (!shruneImage || !djReference) return;

        // Update sprite
        shruneImage.sprite = djReference.LeftTrack.Glyph.Sprite;

        // Clamp and normalize value 0 → 1
        float normalized = Mathf.InverseLerp(min, max, value);

        // Update alpha based on normalized value
        Color mainColor = shruneImage.color;
        mainColor.a = Mathf.Clamp01(normalized);
        shruneImage.color = mainColor;

        Debug.Log(normalized);
        // Update fill bar if present
        if (shruneFillImage)
        {
            shruneFillImage.fillAmount = normalized;
            //shruneFillImage.color = new Color(
            //    shruneFillImage.color.r,
            //    shruneFillImage.color.g,
            //    shruneFillImage.color.b,
            //    Mathf.Clamp01(normalized)
            //);
        }
    }
}
