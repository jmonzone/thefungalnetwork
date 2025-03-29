using UnityEngine;
using UnityEngine.Events;

public class FungalDash : Ability
{
    public event UnityAction OnDashStart;
    public event UnityAction OnDashComplete;

    public override void CastAbility(Vector3 targetPosition)
    {
        //Debug.Log($"Dash {gameObject.name}");
        OnDashStart?.Invoke();
        networkFungal.Dash(targetPosition, OnDashComplete);
        StartCoroutine(Cooldown.StartCooldown());
    }

    public override Vector3 DefaultTargetPosition => transform.position + transform.forward * range;
    public override bool UseTrajectory => false;
}
