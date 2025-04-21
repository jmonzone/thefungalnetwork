using UnityEngine;

[CreateAssetMenu(menuName = "Fish/Ability/Bubble")]
public class BubbleAbility : DirectionalAbility, IMovementAbility
{
    [SerializeField] private BubbleController bubblePrefab;
    [SerializeField] private float range = 4f;

    public override Vector3 DefaultTargetPosition => Fungal.transform.position + Fungal.transform.forward * range;

    public override bool UseTrajectory => true;

    public override float Range => range;

    public override void Initialize(FungalController fungal)
    {
        base.Initialize(fungal);
    }

    public override void CastAbility(Vector3 targetPosition)
    {
        base.CastAbility(targetPosition);

        var bubble = Instantiate(bubblePrefab, targetPosition, Quaternion.identity);
    }
}
