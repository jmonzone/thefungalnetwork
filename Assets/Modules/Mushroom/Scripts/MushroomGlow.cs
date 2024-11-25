using System.Collections;
using UnityEngine;

public class MushroomGlow : MonoBehaviour
{
    [SerializeField] private Light light;

    private void OnCollisionEnter(Collision collision)
    {
        var projectile = collision.gameObject.GetComponentInParent<Projectile>();
        if (projectile)
        {
            Debug.Log("Hit");

            light.intensity = 0f;
            StartCoroutine(HitAnimation());
        }
    }

    private IEnumerator HitAnimation()
    {
        float elapsedTime = 0f;
        float growthMultiplier = 5f;       // Maximum growth factor
        float quickGrowthDuration = 0.25f;    // Time for the quick growth phase
        float animationDuration = 0.75f;      // Total time for the animation

        // Quick growth phase
        while (elapsedTime < quickGrowthDuration)
        {
            float normalizedTime = elapsedTime / quickGrowthDuration;

            // Ease-out growth for a quick, satisfying expansion
            float scaleValue = Mathf.Lerp(0f, growthMultiplier, Mathf.Sin(normalizedTime * Mathf.PI * 0.5f));
            light.intensity = scaleValue;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0;

        // Shrink and bounce phase
        while (elapsedTime < animationDuration)
        {
            float normalizedTime = elapsedTime / animationDuration;

            // Shrink with an ease-in curve
            float scaleValue = Mathf.Lerp(growthMultiplier, 0f, 1 - Mathf.Cos(normalizedTime * Mathf.PI * 0.5f));
            light.intensity = scaleValue;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
