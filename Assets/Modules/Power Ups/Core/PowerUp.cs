using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private Ability ability;
    [SerializeField] private float resetDelay = 3f; // seconds until it resets
    [SerializeField] private Movement render;

    private float detectionRadius = 1.5f;
    private bool hasBeenCollected = false;

    private void Awake()
    {
        if (!ability) Debug.LogWarning("No ability on power up?");
    }

    void Update()
    {
        if (!ability) return;
        if (hasBeenCollected) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider hit in hits)
        {
            var fungal = hit.GetComponentInParent<FungalController>();
            if (fungal && fungal.TryApplyAbility(ability))
            {
                hasBeenCollected = true;
                StartCoroutine(ResetAfterDelay());
                break;
            }
        }
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return render.ScaleOverTime(0.25f, 0f);
        render.gameObject.SetActive(false);
        yield return new WaitForSeconds(resetDelay);
        render.gameObject.SetActive(true);
        yield return render.ScaleOverTime(0.25f, 1f);
        hasBeenCollected = false;
    }
}
