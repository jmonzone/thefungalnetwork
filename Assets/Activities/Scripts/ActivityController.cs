using UnityEngine;

public abstract class ActivityController : MonoBehaviour
{
    [SerializeField] private ActivityReference activity;
    [SerializeField] private Skill primarySkill;

    protected ActivityReference Activity => activity;
    protected Skill PrimarySkill => primarySkill;

    protected virtual void Awake()
    {
    }

    private void OnEnable()
    {
        activity.OnActivityHasStarted += OnActivityStart;
        activity.OnActivityHasEnded += OnActivityEnded;
        activity.OnUnitEnter += OnUnitEnter;
        activity.OnUnitExit += OnUnitExit;
        activity.OnPlayerEnter += OnPlayerEnter;
        activity.OnPlayerExit += OnPlayerExit;
    }

    private void OnDisable()
    {
        activity.OnActivityHasStarted -= OnActivityStart;
        activity.OnActivityHasEnded -= OnActivityEnded;
        activity.OnUnitEnter -= OnUnitEnter;
        activity.OnUnitExit -= OnUnitExit;
        activity.OnPlayerEnter -= OnPlayerEnter;
        activity.OnPlayerExit -= OnPlayerExit;
    }

    protected virtual void OnPlayerEnter(PlayerController player)
    {
    }

    protected virtual void OnPlayerExit(PlayerController player)
    {
    }

    protected virtual void OnActivityStart()
    {
    }

    protected virtual void OnActivityEnded()
    {
    }

    protected virtual void OnUnitEnter(UnitController unit)
    {

    }

    protected virtual void OnUnitExit(UnitController unit)
    {

    }
}
