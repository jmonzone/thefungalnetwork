using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PowerUp : MonoBehaviour, IAbilityHolder
{
    [SerializeField] private Ability ability;
    [SerializeField] private float resetDelay = 3f; // seconds until it resets
    [SerializeField] private Movement render;
    [SerializeField] private GameObject placeholder;

    private float detectionRadius = 1.5f;
    private bool hasBeenCollected = false;

    public UnityAction<FungalController> HandleCollection;
    public UnityAction HandleRespawn;

    bool IAbilityHolder.CanBePickedUp(FungalController fungal)
    {
        return ability && fungal.CanApplyAbility(ability) && !hasBeenCollected;
    }

    Vector3 IAbilityHolder.Position => transform.position;

    private void Awake()
    {
        if (ability) AssignAbility(ability);
        if (placeholder) placeholder.SetActive(false);

        HandleCollection = fungal =>
        {
            ApplyCollectLogic(fungal);
            StartRespawn();
        };

        HandleRespawn = ApplyRespawn;
    }

    public void AssignAbility(Ability ability)
    {
        this.ability = ability;
        var icon = Instantiate(ability.Prefab, render.transform);
        icon.transform.localPosition = Vector3.zero;

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
        if (hasBeenCollected) return;

        hasBeenCollected = true;
        fungal.AssignAbility(AbilitySlot.EXTERNAL, ability);
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
