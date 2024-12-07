using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class Projectile : MonoBehaviour
{
    [SerializeField] private MovementController movement;
    [SerializeField] private Transform render;
    [SerializeField] private ParticleSystem projectileParticles;
    [SerializeField] private ParticleSystem dissipateParticles;
    [SerializeField] private Light light;
    [SerializeField] private Controller controller;

    public event UnityAction OnDissipate;
    public event UnityAction OnComplete;

    public void Shoot(Vector3 direction, float maxDistance)
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.one;

        StartCoroutine(WhispySpiralMotion(direction, 3f, maxDistance));
    }

    private IEnumerator WhispySpiralMotion(Vector3 direction, float speed, float maxDistance)
    {
        float elapsedTime = 0f;
        // Animation parameters
        float quickGrowthDuration = 0.5f;    // Time for the quick growth phase
        float growthMultiplier = .75f;       // Maximum growth factor

        Vector3 originalScale = transform.localScale; // Initial scale
        transform.localScale = Vector3.zero;

        // Quick growth phase
        while (elapsedTime < quickGrowthDuration)
        {
            float normalizedTime = elapsedTime / quickGrowthDuration;

            // Ease-out growth for a quick, satisfying expansion
            float scaleValue = Mathf.Lerp(0f, growthMultiplier, Mathf.Sin(normalizedTime * Mathf.PI * 0.5f));
            transform.localScale = originalScale * scaleValue;
            light.intensity = scaleValue;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Vector3 startPosition = transform.position; // Initial position to track distance
        elapsedTime = 0f;

        // Parameters for motion
        float oscillationAmplitude = 1f; // Amplitude of the up-and-down motion
        float oscillationFrequency = 5f;   // Speed of the up-and-down motion
        float spiralRadius = 2f;         // Radius of the spiral motion
        float spiralSpeed = 5f;            // Speed of the spiral rotation
        float scaleVariation = 0.1f;       // Scale pulsing variation

        while (Vector3.Distance(startPosition, transform.position) < maxDistance)
        {
            // Linear motion
            transform.position += direction.normalized * speed * Time.deltaTime;

            // Spiral motion
            float spiralX = Mathf.Cos(elapsedTime * spiralSpeed) * spiralRadius;
            float spiralZ = Mathf.Sin(elapsedTime * spiralSpeed) * spiralRadius;
            transform.position += new Vector3(spiralX, 0f, spiralZ) * Time.deltaTime;

            // Whispy oscillation (up/down motion)
            float offsetY = Mathf.Sin(elapsedTime * oscillationFrequency) * oscillationAmplitude;
            transform.position += Vector3.up * offsetY * Time.deltaTime;

            // Scale pulsing
            float scaleFactor = 1 + Mathf.Sin(elapsedTime * oscillationFrequency * 2) * scaleVariation;
            transform.localScale = originalScale * scaleFactor;
            light.intensity = scaleFactor;

            // Increment elapsed time
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Once max distance is reached, stop motion
        transform.localScale = originalScale; // Reset scale
        yield return Dissipate();
    }

    public void StartDisspate()
    {
        projectileParticles.Stop();
        dissipateParticles.gameObject.SetActive(true);
        dissipateParticles.Play();
    }

    private IEnumerator Dissipate()
    {
        StartDisspate();
        OnDissipate?.Invoke();

        float elapsedTime = 0f;
        float quickGrowthDuration = 0.75f;    // Time for the quick growth phase
        float growthMultiplier = 1.5f;       // Maximum growth factor
        float animationDuration = 0.25f;      // Total time for the animation
        float bounceHeight = 0.5f;           // Maximum bounce height


        Vector3 originalScale = transform.localScale; // Initial scale
        Vector3 originalPosition = transform.position; // Initial position

        // Quick growth phase
        while (elapsedTime < quickGrowthDuration)
        {
            float normalizedTime = elapsedTime / quickGrowthDuration;

            // Ease-out growth for a quick, satisfying expansion
            float scaleValue = Mathf.Lerp(1f, growthMultiplier, Mathf.Sin(normalizedTime * Mathf.PI * 0.5f));
            transform.localScale = originalScale * scaleValue;
            light.intensity = scaleValue;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f; // Reset for shrink and bounce phase

        var volume = controller.Volume;

        // Add Bloom effect if not already present
        if (!volume.profile.TryGet(out Bloom bloom))
        {
            bloom = volume.profile.Add<Bloom>(true);
        }

        // Shrink and bounce phase
        while (elapsedTime < animationDuration)
        {
            float normalizedTime = elapsedTime / animationDuration;

            // Shrink with an ease-in curve
            float scaleValue = Mathf.Lerp(growthMultiplier, 0f, 1 - Mathf.Cos(normalizedTime * Mathf.PI * 0.5f));
            transform.localScale = originalScale * Mathf.Clamp(scaleValue, 0.01f, growthMultiplier);
            light.intensity = scaleValue;

            // Set Bloom intensity
            bloom.intensity.value = scaleValue * 5f;
            bloom.intensity.overrideState = true;

            // Bounce with a diminishing sine wave
            float bounceValue = Mathf.Sin(normalizedTime * Mathf.PI) * bounceHeight * (1 - normalizedTime);
            transform.position = originalPosition + Vector3.up * bounceValue;

            elapsedTime += Time.deltaTime;
            yield return null;
        }



        transform.localScale = Vector3.zero;
        transform.position = originalPosition;

        EndAnimation();
    }

    public void EndAnimation()
    {
        Debug.Log("ending animation");
        // End of animation cleanup
        dissipateParticles.Stop();
        OnComplete?.Invoke();

        Invoke(nameof(Hide), 1f);
    }

    private void Hide()
    {
        gameObject.SetActive(false);

    }

}
