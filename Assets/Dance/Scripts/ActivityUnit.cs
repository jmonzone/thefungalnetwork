using UnityEngine;
using UnityEngine.Events;

// interface between activity and unit controller
public class ActivityUnit : MonoBehaviour
{
    private ActivityReference activity;
    private UnitController controller;
    private Vector3 activityPosition;

    public Vector3 ActivityPosition => activityPosition;
    public bool IsPlayer => controller is PlayerController;
    public bool CanJoinActivity => activity && !IsPlayer;
    public bool CanLeaveActivity => activity && !IsPlayer;

    public string Name => controller.Instance.Data.Name;
    public Sprite Sprite => controller.Instance.Data.Sprite;
    public Color Color => controller.Color;
    public UnitSkill Skill => controller.Instance.Skills[activity.PrimarySkill];

    public event UnityAction<ActivityUnit,float> OnXPIncreased;

    private void Awake()
    {
        controller = GetComponent<UnitController>();
    }

    public void JoinActivity(ActivityReference activity)
    {
        this.activity = activity;
        controller.SetLookPosition(activity.Origin);
    }

    public void ExitActivity()
    {
        controller.ApplyDefaultBehaviour();
    }

    public void SetBehaviour(ActivityBehaviour behaviour)
    {
        controller.SetBehaviour(behaviour);
    }

    public void UpdatePosition(Vector3 position)
    {
        activityPosition = position;
        controller.SetDestination(position);
    }

    public void IncreaseXP(float value)
    {
        Debug.Log("ActivityUnit.IncreaseXP");
        controller.Instance.Skills[activity.PrimarySkill].IncreaseSkillXP(value);
        OnXPIncreased?.Invoke(this, value);
    }
}
