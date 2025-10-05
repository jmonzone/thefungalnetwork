using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class ActivityReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference activityView;
    [SerializeField] private Skill primarySkill;

    [Header("Runtime")]
    [SerializeField] private Vector3 origin;
    [SerializeField] private List<ActivityUnit> units = new List<ActivityUnit>();

    public Vector3 Origin => origin;
    public Skill PrimarySkill => primarySkill;
    public List<ActivityUnit> Units => units;

    public event UnityAction OnActivityHasStarted;
    public event UnityAction OnActivityHasEnded;

    public event UnityAction<ActivityUnit> OnUnitEnter;
    public event UnityAction<ActivityUnit> OnUnitExit;

    public event UnityAction<OnXpIncreasedEventArgs> OnXPIncreased;

    public event UnityAction<ActivityUnit> OnPlayerEnter;
    public event UnityAction<ActivityUnit> OnPlayerExit;

    public void StartActivity(Vector3 origin, List<UnitController> units)
    {
        Debug.Log($"Starting activity {name}");
        this.origin = origin;
        this.units = new List<ActivityUnit>();

        foreach(var unit in units)
        {
            AddUnit(unit.GetComponent<ActivityUnit>());
        }

        OnActivityHasStarted?.Invoke();
    }

    public void EndActivity()
    {
        OnActivityHasEnded?.Invoke();
        units = new List<ActivityUnit>();
    }

    public void EnterActivity(ActivityUnit player)
    {
        AddUnit(player);
        navigation.Navigate(activityView);
        OnPlayerEnter?.Invoke(player);
    }

    public void ExitActivity(ActivityUnit player)
    {
        RemoveUnit(player);
        navigation.GoBackToRoot();
        OnPlayerExit?.Invoke(player);
    }

    public void RemoveUnit(ActivityUnit unit)
    {
        unit.ExitActivity();
        units.Remove(unit);
        unit.OnXPIncreased -= Unit_OnXPIncreased;
        UpdateUnits();
        OnUnitExit?.Invoke(unit);
    }

    public void AddUnit(ActivityUnit unit)
    {
        unit.JoinActivity(this);
        units.Add(unit);
        unit.OnXPIncreased += Unit_OnXPIncreased;
        UpdateUnits();
        OnUnitEnter?.Invoke(unit);
    }

    private void Unit_OnXPIncreased(OnXpIncreasedEventArgs args)
    {
        OnXPIncreased?.Invoke(args);
    }

    private void UpdateUnits()
    {
        int count = units.Count;
        var offset = Random.Range(0, Mathf.PI * 2);

        for (int i = 0; i < count; i++)
        {
            // Evenly spaced angle around circle, but clockwise
            float angle = -(i / (float)count) * Mathf.PI * 2f + offset;

            // Direction from center (clockwise order)
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            // Position offset outward from center
            Vector3 destination = origin + direction * 1f;

            units[i].UpdatePosition(destination);
        }
    }
}