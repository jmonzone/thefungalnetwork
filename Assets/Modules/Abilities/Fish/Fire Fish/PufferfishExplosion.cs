using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PufferfishExplosion : MonoBehaviour
{
    [SerializeField] private GameReference pufferball;
    [SerializeField] private Movement movement;
    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] private AudioClip explosionClip;

    private FishController fish;
    private AudioSource audioSource;
    private HitDetector hitDetector;

    public event UnityAction OnExplodeComplete;

    private void Awake()
    {
        fish = GetComponent<FishController>();
        audioSource = GetComponent<AudioSource>();
        hitDetector = GetComponent<HitDetector>();
    }

    public void EnableDamage(float damage)
    {
        StartCoroutine(DamageRoutine(damage));
    }

    //todo: reuselogic across bubble, wind fish and pufferfish
    private IEnumerator DamageRoutine(float damage)
    {
        List<Collider> hits = new List<Collider>();

        while (movement.gameObject.activeSelf)
        {
            hitDetector.CheckHits(movement.transform.localScale.x / 2f, hit =>
            {
                if (hits.Contains(hit)) return;

                var fungal = hit.GetComponent<FungalController>();
                if (fungal != null && !fungal.IsDead)
                {
                    fungal.ModifySpeed(0, 0.5f, false);
                    fungal.Health.Damage(damage, fish.Fungal.Id);
                    hits.Add(hit);
                    return;
                }

                var bubble = hit.GetComponentInParent<NetworkBubble>();
                if (bubble != null)
                {
                    bubble.PopServerRpc();
                    hits.Add(hit);
                    return;
                }
            });

            yield return null;
        }
    }

    public void StartExplosionAnimation(float radius = 1f)
    {
        audioSource.clip = explosionClip;
        audioSource.pitch = 1.5f;
        audioSource.Play();

        StartCoroutine(ExplosionRoutine(radius));
    }

    public IEnumerator ExplosionRoutine(float radius)
    {
        movement.gameObject.SetActive(true);
        particleSystem.Play();
        yield return movement.ScaleOverTime(0.2f, 0, 2f * radius);
        OnExplodeComplete?.Invoke();
        yield return new WaitForSeconds(0.5f);
        GetComponent<TelegraphTrajectory>().HideIndicator();
        yield return movement.ScaleOverTime(0.1f, 2f * radius, 0);
        movement.gameObject.SetActive(false);
        particleSystem.Stop();
    }
}
