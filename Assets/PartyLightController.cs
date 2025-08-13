using System.Collections;
using UnityEngine;

public class PartyLightController : MonoBehaviour
{
    public Transform lookRotation;      // pivot of the light
    public float rotationSpeed = 50f;   // horizontal speed
    public float verticalAmplitude = 0.2f; // vertical oscillation multiplier
    [Range(0f, 1f)]
    public float phaseOffset = 0f;      // 0..1 to stagger lights


    public Gradient raveColors;       // color gradient to pick from over time
    public float colorChangeSpeed = 2f;

    private LineRenderer lr;
    private float colorTime;

    public bool Enabled => lr.enabled;

    void Start()
    {
        lr = GetComponentInChildren<LineRenderer>();
        lr.enabled = false;

        // Default gradient if none provided
        if (raveColors == null || raveColors.colorKeys.Length == 0)
        {
            raveColors = new Gradient();
            raveColors.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.red, 0f),
                    new GradientColorKey(Color.magenta, 0.25f),
                    new GradientColorKey(Color.blue, 0.5f),
                    new GradientColorKey(Color.green, 0.75f),
                    new GradientColorKey(Color.yellow, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                }
            );
        }
    }

    void Update()
    {
        // 1. Rotate like a moving stage light
        if (lookRotation)
        {
            // horizontal rotation
            lookRotation.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            // vertical oscillation with phase offset
            float vertical = Mathf.Sin((Time.time + phaseOffset) * rotationSpeed * 0.02f) * verticalAmplitude;
            lookRotation.localRotation = Quaternion.Euler(vertical * rotationSpeed, lookRotation.localEulerAngles.y, 0f);
        }

        // 2. Cycle colors over time
        colorTime += Time.deltaTime * colorChangeSpeed;
        if (colorTime > 1f) colorTime -= 1f;
        Color c = raveColors.Evaluate(Mathf.PingPong(colorTime, 1f));
        lr.startColor = c;
        lr.endColor = c;
    }

    public void SetEnabled(bool enabled)
    {
        lr.enabled = enabled;
    }
}
