using System;
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

    [SerializeField] private float oscillationAmplitude = 1f; // Amplitude of the up-and-down motion
    [SerializeField] private float oscillationFrequency = 5f;   // Speed of the up-and-down motion
    [SerializeField] private float spiralRadius = 2f;         // Radius of the spiral motion
    [SerializeField] private float spiralSpeed = 5f;            // Speed of the spiral rotation
    [SerializeField] private float scaleVariation = 0.1f;       // Scale pulsing variation

    private Coroutine whispySpiralMotionCoroutine;

    public event UnityAction OnDissipateStart;
    public event UnityAction<float> OnDissipateUpdate;
    public event UnityAction OnComplete;

    private int hitCount = 0;

    private void Update()
    {
        if (hitCount > 0) return;

        var colliders = Physics.OverlapSphere(transform.position, 0.5f);

        if (colliders.Length > 0)
        {
            foreach(var collider in colliders)
            {
                var attackable = collider.GetComponentInParent<Attackable>();
                if (attackable && isValidTarget != null && isValidTarget(attackable))
                {
                    hitCount++;
                    attackable.RequestDamage();

                    if (whispySpiralMotionCoroutine != null)
                    {
                        StopCoroutine(whispySpiralMotionCoroutine);
                        StartCoroutine(Dissipate());
                    }
                }
            }
        }
    }

    private Func<Attackable, bool> isValidTarget;

    public void Shoot(Vector3 direction, float maxDistance, float speed, Func<Attackable, bool> isValidTarget)
    {
        this.isValidTarget = isValidTarget;
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
        whispySpiralMotionCoroutine = StartCoroutine(WhispySpiralMotion(direction, speed, maxDistance));
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
        

        while (Vector3.Distance(startPosition, transform.position) < maxDistance)
        {
            // Linear motion
            transform.position += speed * Time.deltaTime * direction.normalized;

            // Spiral motion
            float spiralX = Mathf.Cos(elapsedTime * spiralSpeed) * spiralRadius;
            float spiralZ = Mathf.Sin(elapsedTime * spiralSpeed) * spiralRadius;
            transform.position += speed * Time.deltaTime * new Vector3(spiralX, 0f, spiralZ);

            // Whispy oscillation (up/down motion)
            float offsetY = Mathf.Sin(elapsedTime * oscillationFrequency) * oscillationAmplitude;
            transform.position += offsetY * Time.deltaTime * Vector3.up ;

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

    public void HideProjectileParticles()
    {
        projectileParticles.Stop();
    }

    public void StartDisspate()
    {
        HideProjectileParticles();
        dissipateParticles.gameObject.SetActive(true);
        dissipateParticles.Play();
    }

    private IEnumerator Dissipate()
    {
        whispySpiralMotionCoroutine = null;

        StartDisspate();
        OnDissipateStart?.Invoke();

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
            OnDissipateUpdate?.Invoke(elapsedTime);

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

        Invoke(nameof(Hide), 2f);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
