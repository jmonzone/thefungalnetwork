using UnityEngine;
using UnityEngine.Events;

public class PufferfishTemper : MonoBehaviour
{
    [SerializeField] private Renderer renderer;

    [SerializeField] private float maxTemperDuration = 5f; // Time to reach max temper
    [SerializeField] private int flashCount = 3; // Number of flashes at max temper
    [SerializeField] private float flashDuration = 1f; // Duration of the flashing phase in seconds

    public float Temper { get; private set; }
    private bool isIncreasing;
    public event UnityAction OnMaxTemperReached;

    private void Update()
    {
        if (isIncreasing)
        {
            float temperIncreaseSpeed = 1f / maxTemperDuration;
            SetTemper(Temper + temperIncreaseSpeed * Time.deltaTime);

            if (Temper >= 1f)
            {
                Temper = 1f;
                isIncreasing = false;
                OnMaxTemperReached?.Invoke(); // Trigger event when full
            }
        }

        UpdateColor();
    }

    private void UpdateColor()
    {
        if (Temper >= 1f - (flashDuration / maxTemperDuration)) // Only flash for the last part
        {
            float elapsedFlashTime = (Temper - (1f - flashDuration / maxTemperDuration)) / (flashDuration / maxTemperDuration);
            float flashSpeed = flashCount * Mathf.PI * 2f; // Frequency based on flash count
            float flash = (Mathf.Sin(elapsedFlashTime * flashSpeed) + 1f) / 2f; // Smooth pulsing effect

            renderer.material.color = Color.Lerp(Color.red, Color.yellow, flash);
        }
        else
        {
            // Normal transition from yellow to red
            renderer.material.color = Color.Lerp(Color.yellow, Color.red, Temper / (1f - flashDuration / maxTemperDuration));
        }
    }

    public void StartTemper()
    {
        isIncreasing = true;
    }

    public void StopTimer()
    {
        isIncreasing = false;
        Temper = 0f;
        UpdateColor();
    }

    public void SetTemper(float value)
    {
        if (Temper == value) return;

        Temper = Mathf.Clamp(value, 0, 1);
        UpdateColor();

        if (Temper == 1)
        {
            OnMaxTemperReached?.Invoke();
        }
    }
}
