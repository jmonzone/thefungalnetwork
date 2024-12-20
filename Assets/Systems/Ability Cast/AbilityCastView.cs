using UnityEngine;

public class AbilityCastView : MonoBehaviour
{
    [SerializeField] private AbilityCast abilityCast;
    [SerializeField] private ShruneItem shrune;

    private void OnEnable()
    {
        abilityCast.OnCastStart += AbilityCast_OnComplete;
    }

    private void OnDisable()
    {
        abilityCast.OnCastStart -= AbilityCast_OnComplete;
    }

    private void AbilityCast_OnComplete()
    {
        var projectile = Instantiate(shrune.ProjectilePrefab, transform.position + Vector3.up * 0.5f, Quaternion.LookRotation(Vector3.forward));
        projectile.Shoot(abilityCast.Direction, shrune.MaxDistance, shrune.Speed, abilityCast.IsValidTarget);
    }
}
