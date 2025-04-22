using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IProjectile
{
    public void Initialize(Vector3 targetPosition);
    public event UnityAction<Vector3> OnInitialized;
}

public class PufferfishExplosion : MonoBehaviour, IProjectile
{
    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] private AudioClip explosionClip;

    private Movement movement;
    private AudioSource audioSource;
    private HitDetector hitDetector;

    public event UnityAction OnExplodeComplete;
    public event UnityAction<Vector3> OnInitialized;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        audioSource = GetComponent<AudioSource>();
        hitDetector = GetComponent<HitDetector>();
    }

    public void Initialize(Vector3 targetPosition)
    {
        movement.SetTrajectoryMovement(targetPosition);
        GetComponent<TelegraphTrajectory>().ShowIndicator(targetPosition, 1f);
    }

    private void OnEnable()
    {
        movement.OnDestinationReached += Movement_OnDestinationReached; ;
    }

    private void OnDisable()
    {
        movement.OnDestinationReached -= Movement_OnDestinationReached;
    }

    private void Movement_OnDestinationReached()
    {
        StartExplosionAnimation();
    }

    public void EnableDamage(float damage, ulong id)
    {
        StartCoroutine(DamageRoutine(damage, id));
    }

    //todo: reuselogic across bubble, wind fish and pufferfish
    private IEnumerator DamageRoutine(float damage, ulong id)
    {
        List<Collider> hits = new List<Collider>();

        while (gameObject.activeSelf)
        {
            hitDetector.CheckHits(movement.transform.localScale.x / 2f, hit =>
            {
                if (hits.Contains(hit)) return;

                var fungal = hit.GetComponent<FungalController>();
                if (fungal != null && !fungal.IsDead)
                {
                    //fungal.ModifySpeedServerRpc(0, 0.5f);
                    fungal.Health.Damage(damage, id);
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
        gameObject.SetActive(true);
        particleSystem.Play();
        yield return movement.ScaleOverTime(0.2f, 0, 2f * radius);
        OnExplodeComplete?.Invoke();
        yield return new WaitForSeconds(0.5f);
        GetComponent<TelegraphTrajectory>().HideIndicator();
        yield return movement.ScaleOverTime(0.1f, 2f * radius, 0);
        gameObject.SetActive(false);
        particleSystem.Stop();
    }
}
