using System.Collections;
using UnityEngine;

public class ParticleColorManager : MonoBehaviour
{
    private ParticleSystem particles;
    private Coroutine colorChangeCoroutine;

    private void Awake()
    {
        particles = GetComponentInChildren<ParticleSystem>();
    }

    public void ChangeColor(Gradient targetGradient, float duration, System.Action onComplete = null)
    {
        if (colorChangeCoroutine != null)
        {
            StopCoroutine(colorChangeCoroutine);
        }

        colorChangeCoroutine = StartCoroutine(LerpGradient(targetGradient, duration, onComplete));
    }

    private IEnumerator LerpGradient(Gradient targetGradient, float duration, System.Action onComplete)
    {
        var colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient currentGradient = colorOverLifetime.color.gradient;
        Gradient lerpedGradient = new Gradient();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float progress = t / duration;

            GradientColorKey[] colorKeys = LerpColorKeys(currentGradient.colorKeys, targetGradient.colorKeys, progress);
            GradientAlphaKey[] alphaKeys = LerpAlphaKeys(currentGradient.alphaKeys, targetGradient.alphaKeys, progress);

            lerpedGradient.SetKeys(colorKeys, alphaKeys);

            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(lerpedGradient);

            yield return null;
        }

        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(targetGradient);
        onComplete?.Invoke();
    }

    private GradientColorKey[] LerpColorKeys(GradientColorKey[] fromKeys, GradientColorKey[] toKeys, float t)
    {
        int keyCount = Mathf.Min(fromKeys.Length, toKeys.Length);
        GradientColorKey[] resultKeys = new GradientColorKey[keyCount];

        for (int i = 0; i < keyCount; i++)
        {
            resultKeys[i] = new GradientColorKey(
                Color.Lerp(fromKeys[i].color, toKeys[i].color, t),
                Mathf.Lerp(fromKeys[i].time, toKeys[i].time, t)
            );
        }

        return resultKeys;
    }

    private GradientAlphaKey[] LerpAlphaKeys(GradientAlphaKey[] fromKeys, GradientAlphaKey[] toKeys, float t)
    {
        int keyCount = Mathf.Min(fromKeys.Length, toKeys.Length);
        GradientAlphaKey[] resultKeys = new GradientAlphaKey[keyCount];

        for (int i = 0; i < keyCount; i++)
        {
            resultKeys[i] = new GradientAlphaKey(
                Mathf.Lerp(fromKeys[i].alpha, toKeys[i].alpha, t),
                Mathf.Lerp(fromKeys[i].time, toKeys[i].time, t)
            );
        }

        return resultKeys;
    }
}
