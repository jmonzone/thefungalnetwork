using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ValueBarParticleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform particleContainer;
    [SerializeField] private RectTransform targetUI;
    [SerializeField] private ValueParticleController particlePrefab;

    [Header("Settings")]
    [SerializeField] private Color startColor;
    [SerializeField] private Color targetColor;

    private List<ValueParticleController> activeParticles = new List<ValueParticleController>();

    public event UnityAction OnParticleReached;

    private void OnDisable()
    {
        foreach(var particle in activeParticles)
        {
            Destroy(particle);
        }

        activeParticles = new List<ValueParticleController>();
    }

    public void SetStartColor(Color color)
    {
        startColor = color;
        foreach (var particle in activeParticles)
        {
            particle.startColor = color;
        }
    }

    public void SetTargetColor(Color color)
    {
        targetColor = color;
        foreach(var particle in activeParticles)
        {
            particle.targetColor = color;
        }
    }

    public void BurstFromWorld(int count, Vector3 screenPos, UnityAction onComplete)
    {
        for (int i = 0; i < count; i++)
        {
            var p = Instantiate(particlePrefab, particleContainer);
            p.Initialize(screenPos, targetUI, () =>
            {
                activeParticles.Remove(p);
                Destroy(p.gameObject);
                OnParticleReached?.Invoke();
                if (activeParticles.Count == 0)
                {
                    onComplete?.Invoke();
                }
            });

            p.startColor = startColor;
            p.targetColor = targetColor;
            activeParticles.Add(p);
        }
    }
}
