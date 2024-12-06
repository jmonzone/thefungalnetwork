using UnityEngine;

public class AbilityCastController : MonoBehaviour
{
    [SerializeField] private AbilityCast abilityCast;
    [SerializeField] private ShruneCollection shruneCollection;

    private void OnEnable()
    {
        abilityCast.OnComplete += CastAbility;
    }

    private void OnDisable()
    {
        abilityCast.OnComplete -= CastAbility;
    }

    private void CastAbility()
    {
        if (shruneCollection.TryGetShruneById(abilityCast.ShruneId, out ShruneItem shrune))
        {
            var projectile = Instantiate(shrune.ProjectilePrefab, abilityCast.StartPosition, Quaternion.LookRotation(abilityCast.Direction), transform);
            projectile.Shoot(abilityCast.Direction, abilityCast.MaxDistance);
        }
    }
}
