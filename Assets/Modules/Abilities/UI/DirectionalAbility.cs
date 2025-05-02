using UnityEngine;

public abstract class DirectionalAbility : Ability
{
    public abstract float Range { get; }
    public abstract bool UseTrajectory { get; }

    public void CastAbility(Vector3 targetPosition)
    {
        base.CastAbility();

        if (UseTrajectory) OnAbilityCasted(targetPosition);
        else
        {
            var direction = targetPosition - Fungal.transform.position;
            var fixedPosition = Fungal.transform.position + direction.normalized * Range;
            OnAbilityCasted(fixedPosition);
        }
    }

    protected abstract void OnAbilityCasted(Vector3 targetPosition);


    public virtual Vector3 DefaultTargetPosition
    {
        get
        {
            Vector3 origin = Fungal.transform.position;
            Vector3 forwardTarget = origin + Fungal.transform.forward * Range;
            float searchRadius = Range;
            LayerMask targetLayer = ~0;

            Collider[] colliders = Physics.OverlapSphere(origin, searchRadius, targetLayer);
            FungalController closestFungal = null;
            float closestDistance = Mathf.Infinity;

            foreach (var collider in colliders)
            {
                FungalController fungal = collider.GetComponent<FungalController>();
                if (fungal == null) continue;
                if (fungal.IsDead) continue;
                if (Fungal == fungal) continue;

                float distance = Vector3.Distance(origin, fungal.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestFungal = fungal;
                }
            }

            var targetPosition = closestFungal != null ? closestFungal.transform.position : forwardTarget;
            targetPosition.y = 0;
            return targetPosition;
        }
    }
}
