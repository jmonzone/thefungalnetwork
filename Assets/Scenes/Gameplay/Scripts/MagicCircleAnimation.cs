using UnityEngine;
using UnityEngine.UI;

public class MagicCircleAnimation : MonoBehaviour
{
    [SerializeField] private Image image;

    public float pulseSpeed = 2f;
    public Color highlightColorA = Color.cyan;  // Dimmer or base pulse
    public Color highlightColorB = Color.white; // Brighter or target pulse

    private void Update()
    {
        float t = (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) + 1f) / 2f;
        Color pulsingColor = Color.Lerp(highlightColorA, highlightColorB, t);
        image.color = pulsingColor;
    }
}
