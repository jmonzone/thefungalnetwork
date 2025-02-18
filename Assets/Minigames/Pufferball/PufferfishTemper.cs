using UnityEngine;
using UnityEngine.Events;

public class PufferfishTemper : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    [SerializeField] private float temperIncreaseSpeed = 1f;

    public float Temper { get; private set; }
    private bool isIncreasing;

    public event UnityAction OnMaxTemperReached;

    private void Update()
    {
        if (isIncreasing)
        {
            Temper += temperIncreaseSpeed * Time.deltaTime;
            Temper = Mathf.Clamp(Temper, 0, 1);
            UpdateColor();

            if (Temper == 1)
            {
                isIncreasing = false;
                OnMaxTemperReached?.Invoke();
            }
        }
    }

    private void UpdateColor()
    {
        renderer.material.color = Color.Lerp(Color.yellow, Color.red, Temper);
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
        Temper = Mathf.Clamp(value, 0, 1);
        UpdateColor();
    }
}
