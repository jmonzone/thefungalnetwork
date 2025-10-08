using UnityEngine;
using UnityEngine.Events;

public struct OnXpIncreasedEventArgs
{
    public ActivityUnit Unit;
    public float XP;
    public Vector3 SourcePosition;
}

// interface between activity and unit controller
public class ActivityUnit : MonoBehaviour
{
    private ActivityReference activity;
    private UnitController controller;
    private Vector3 activityPosition;

    public UnitController Controller => controller;

    public Vector3 ActivityPosition => activityPosition;
    public Quaternion LookRotation => Quaternion.LookRotation(controller.LookPosition - transform.position);
    public bool IsPlayer => controller is PlayerController;
    public bool CanJoinActivity => activity && !IsPlayer;
    public bool CanLeaveActivity => activity && !IsPlayer;

    public string Name => controller.Instance.Data.Name;
    public Sprite Sprite => controller.Instance.Data.Sprite;
    public Color Color => controller.Color;
    public UnitSkill Skill => controller.Instance.Skills[activity.PrimarySkill];

    public event UnityAction<OnXpIncreasedEventArgs> OnXPIncreased;

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

    public void LookAt(Vector3 target)
    {
        controller.SetLookPosition(target);
    }

    public void IncreaseXP(float value, Vector3 sourcePosition)
    {
        //Debug.Log("ActivityUnit.IncreaseXP");
        controller.Instance.Skills[activity.PrimarySkill].IncreaseSkillXP(value);
        OnXPIncreased?.Invoke(new OnXpIncreasedEventArgs
        {
            Unit = this,
            XP = value,
            SourcePosition = sourcePosition
        });
    }
}
