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
    [SerializeField] private List<UnitController> units = new List<UnitController>();

    public Vector3 Origin => origin;
    public Skill PrimarySkill => primarySkill;
    public List<UnitController> Units => units;

    public event UnityAction OnActivityHasStarted;
    public event UnityAction OnActivityHasEnded;

    public event UnityAction<UnitController> OnUnitEnter;
    public event UnityAction<UnitController> OnUnitExit;

    public event UnityAction<UnitController, float> OnUnitXpIncreased;

    public event UnityAction<PlayerController> OnPlayerEnter;
    public event UnityAction<PlayerController> OnPlayerExit;

    public void StartActivity(Vector3 origin, List<UnitController> units)
    {
        Debug.Log($"Starting activity {name}");
        this.units = units;
        this.origin = origin;
        OnActivityHasStarted?.Invoke();
    }

    public void EndActivity()
    {
        OnActivityHasEnded?.Invoke();
        units = new List<UnitController>();
    }

    public void EnterActivity(PlayerController player)
    {
        AddUnit(player);
        navigation.Navigate(activityView);
        OnPlayerEnter?.Invoke(player);
    }

    public void ExitActivity(PlayerController player)
    {
        RemoveUnit(player);
        navigation.GoBackToRoot();
        OnPlayerExit?.Invoke(player);
    }

    public void RemoveUnit(UnitController unit)
    {
        units.Remove(unit);
        UpdateUnits();
        OnUnitExit?.Invoke(unit);
    }

    public void AddUnit(UnitController unit)
    {
        Debug.Log("AddUnit");
        units.Add(unit);
        UpdateUnits();
        OnUnitEnter?.Invoke(unit);
    }

    public void IncreaseXP(UnitController unit, float value)
    {
        if (Units.Contains(unit))
        {
            unit.Instance.Skills[primarySkill].IncreaseSkillXP(value);
            OnUnitXpIncreased?.Invoke(unit, value);
        }
    }

    private void UpdateUnits()
    {
        Debug.Log("UpdateUnits");

        int count = units.Count;

        for (int i = 0; i < count; i++)
        {
            // Evenly spaced angle around circle, but clockwise
            float angle = -(i / (float)count) * Mathf.PI * 2f;

            // Direction from center (clockwise order)
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            // Position offset outward from center
            Vector3 destination = origin + direction * 1f;

            units[i].GetComponent<UnitDance>().SetOriginalPosition(destination);
            units[i].SetDestination(destination);
            units[i].SetLookPosition(origin);
        }
    }
}