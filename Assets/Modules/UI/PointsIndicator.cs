using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsIndicator : MonoBehaviour
{
    [SerializeField] private GameReference pufferballReference;
    [SerializeField] private Transform pointsTextAnchor;
    [SerializeField] private List<Color> colors;

    public TMPro.TextMeshProUGUI popupPrefab; // Reference to the Text prefab
    public int poolSize = 10; // Number of popups to pool
    private Queue<TMPro.TextMeshProUGUI> popupPool; // The pool
    private List<TMPro.TextMeshProUGUI> activePopups; // Active popups in the scene

    private void Awake()
    {
        // Initialize the pools
        popupPool = new Queue<TMPro.TextMeshProUGUI>();
        activePopups = new List<TMPro.TextMeshProUGUI>();

        // Populate the pool with new popup objects
        for (int i = 0; i < poolSize; i++)
        {
            var popup = Instantiate(popupPrefab, pointsTextAnchor);
            popup.gameObject.SetActive(false);
            popupPool.Enqueue(popup);
        }

        pufferballReference.OnClientPlayerAdded += PufferballReference_OnClientPlayerAdded;
    }

    private void PufferballReference_OnClientPlayerAdded()
    {
        pufferballReference.OnClientPlayerAdded -= PufferballReference_OnClientPlayerAdded;
        pufferballReference.ClientPlayer.Fungal.OnScoreTriggered += ShowPopup;
    }

    // Function to spawn a new popup
    public void ShowPopup(ScoreEventArgs arg0)
    {
        if (popupPool.Count > 0)
        {
            float duration = 1.5f;
            float moveDistance = 1f;

            // Get the popup from the pool
            var popup = popupPool.Dequeue();
            popup.gameObject.SetActive(true);
            popup.text = $"+{arg0.value} {arg0.label}";
            popup.color = Color.Lerp(colors[0], colors[colors.Count - 1], arg0.value / 250);

            // Start the floating and fading coroutine
            StartCoroutine(FloatUpAndFade(popup, arg0.position, duration, moveDistance));

            // Add it to the active popups list
            activePopups.Add(popup);
        }
    }

    // Coroutine for moving and fading the popup
    private IEnumerator FloatUpAndFade(TMPro.TextMeshProUGUI text, Vector3 worldPosition, float duration, float moveDistance)
    {
        float elapsed = 0f;
        Color startColor = text.color;
        Vector3 offset = Vector3.up * moveDistance;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Calculate the new world position with an upward offset
            Vector3 currentWorldPos = worldPosition + (offset * progress);

            // Convert world position to screen space
            Vector3 screenPos = Camera.main.WorldToScreenPoint(currentWorldPos);

            // Apply the screen position to the RectTransform of the popup text
            text.rectTransform.position = screenPos;

            // Fade out over time
            Color newColor = startColor;
            newColor.a = Mathf.Lerp(1f, 0f, progress);
            text.color = newColor;

            yield return null;
        }

        // After fading, reset and return it to the pool
        text.gameObject.SetActive(false);
        popupPool.Enqueue(text);
        activePopups.Remove(text);
    }

}