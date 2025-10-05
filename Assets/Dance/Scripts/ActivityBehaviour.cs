using UnityEngine;

// interface between ActivityBehaviour and Activity Unit
[RequireComponent(typeof(ActivityUnit))]
public abstract class ActivityBehaviour : UnitBehaviour
{
    [SerializeField] private ActivityReference activityReference;

    private ActivityUnit unit;

    protected ActivityReference Activity => activityReference;
    protected Vector3 ActivityPosition => unit.ActivityPosition;
    public bool IsPlayer => unit.IsPlayer;

    protected override void Awake()
    {
        base.Awake();
        unit = GetComponent<ActivityUnit>();
    }

    public void IncreaseXP(float value)
    {
        //Debug.Log("ActivityBehaviour.IncreaseXP");
        unit.IncreaseXP(value);
    }
}
