using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private Ability ability;
    [SerializeField] private float resetDelay = 3f; // seconds until it resets
    [SerializeField] private Movement render;

    private float detectionRadius = 1.5f;
    private bool hasBeenCollected = false;

    public UnityAction<FungalController> HandleCollection;
    public UnityAction HandleRespawn;

    private void Awake()
    {
        if (!ability) Debug.LogWarning("No ability on power up?");
        HandleCollection = fungal =>
        {
            ApplyCollectLogic(fungal);
            StartRespawn();
        };

        HandleRespawn = ApplyRespawn;
    }

    private void Update()
    {
        if (!ability) return;
        if (hasBeenCollected) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider hit in hits)
        {
            var fungal = hit.GetComponentInParent<FungalController>();
            if (fungal && fungal.CanApplyAbility(ability))
            {
                HandleCollection?.Invoke(fungal);
                break;
            }
        }
    }

    public void ApplyCollectLogic(FungalController fungal)
    {
        hasBeenCollected = true;
        fungal.ApplyAbility(ability);
    }

    public void StartRespawn()
    {
        StartCoroutine(ResetAfterDelay());
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return render.ScaleOverTime(0.25f, 0f);
        yield return new WaitForSeconds(resetDelay);
        yield return render.ScaleOverTime(0.25f, 1f);
        HandleRespawn?.Invoke();
    }

    public void ApplyRespawn()
    {
        hasBeenCollected = false;
    }
}
