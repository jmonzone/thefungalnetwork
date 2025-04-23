using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private Ability ability;

    private float detectionRadius = 1.5f;
    private bool hasBeenCollected = false;

    void Update()
    {
        if (hasBeenCollected) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider hit in hits)
        {
            var fungal = hit.GetComponentInParent<FungalController>();
            if (fungal)
            {
                fungal.ApplyAbility(ability);
                hasBeenCollected = true;
                Destroy(gameObject); // Remove the power-up object from the scene
                break;
            }
        }
    }
}
