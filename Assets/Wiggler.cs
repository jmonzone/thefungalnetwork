using UnityEngine;

public class Wiggler : MonoBehaviour
{
    [Header("Wiggle Settings")]
    public float maxOffset = 5f;     // maximum positional offset in pixels
    public float maxRotation = 15f;  // maximum rotation in degrees
    public float wiggleSpeed = 5f;   // how fast it wiggles
    public float randomness = 0.5f;  // adds non-uniform motion

    private Vector3 baseLocalPos;
    private Quaternion baseRotation;
    private Vector3 noiseSeed;

    private void Awake()
    {
        baseLocalPos = transform.localPosition;
        baseRotation = transform.localRotation;
        noiseSeed = new Vector3(
            Random.Range(0f, 100f),
            Random.Range(0f, 100f),
            Random.Range(0f, 100f)
        );
    }

    private void Update()
    {
        float time = Time.time * wiggleSpeed;

        // Chaotic per-axis offsets using Perlin noise
        float offsetX = (Mathf.PerlinNoise(time + noiseSeed.x, 0f) - 0.5f) * 2f * maxOffset;
        float offsetY = (Mathf.PerlinNoise(time + noiseSeed.y, 1f) - 0.5f) * 2f * maxOffset;
        float rotationZ = (Mathf.PerlinNoise(time + noiseSeed.z, 2f) - 0.5f) * 2f * maxRotation;

        transform.localPosition = baseLocalPos + new Vector3(offsetX, offsetY, 0f);
        transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, rotationZ);
    }
}
