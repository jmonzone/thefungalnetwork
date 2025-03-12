using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PufferfishExplosion : MonoBehaviour
{
    [SerializeField] private GameObject radiusIndicator;
    [SerializeField] private Movement movement;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private AudioClip explosionClip;

    private AudioSource audioSource;

    public event UnityAction OnExplodeComplete;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void DealExplosionDamage(float damage, float radius = 1f)
    {
        // Detect all colliders, including triggers
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hitColliders)
        {
            var fungal = hit.GetComponent<NetworkFungal>();
            if (fungal != null)
            {
                fungal.ModifySpeedServerRpc(0, 0.5f);
                fungal.TakeDamageServerRpc(damage);
                continue;
            }

            var bubble = hit.GetComponentInParent<Bubble>();
            if (bubble != null)
            {
                bubble.PopServerRpc();
                continue;
            }
        }
    }

    public void ShowHitIndicator(Vector3 targetPosition, float radius = 1f)
    {
        radiusIndicator.transform.parent = null;
        radiusIndicator.transform.position = targetPosition + Vector3.up * 0.1f;
        radiusIndicator.SetActive(true);
        radiusIndicator.transform.localScale = 2f * radius * Vector3.one;
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
        yield return new WaitForSeconds(0.25f);
        radiusIndicator.SetActive(false);
        yield return movement.ScaleOverTime(0.1f, 2f * radius, 0);
        movement.gameObject.SetActive(false);
        particleSystem.Stop();
    }
}
