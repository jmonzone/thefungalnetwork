using UnityEngine;

public class AbilityCastView : MonoBehaviour
{
    [SerializeField] private AbilityCast abilityCast;

    private void OnEnable()
    {
        abilityCast.OnCast += AbilityCast_OnComplete;
    }

    private void OnDisable()
    {
        abilityCast.OnCast -= AbilityCast_OnComplete;
    }

    private void AbilityCast_OnComplete()
    {
        var shrune = abilityCast.Shrune;
        var projectile = Instantiate(shrune.ProjectilePrefab, transform.position + Vector3.up * 0.5f, Quaternion.LookRotation(Vector3.forward));
        projectile.Shoot(abilityCast.Direction, shrune.MaxDistance, shrune.Speed, abilityCast.IsValidTarget);
    }
}
