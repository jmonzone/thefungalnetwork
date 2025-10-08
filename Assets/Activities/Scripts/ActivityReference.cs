using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class ActivityReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference activityView;
    [SerializeField] private DialogueReference dialogueReference;
    [SerializeField] private Skill primarySkill;

    [Header("Runtime")]
    [SerializeField] private Vector3 origin;
    [SerializeField] private ActivityUnit player;
    [SerializeField] private List<ActivityUnit> units = new List<ActivityUnit>();

    public Vector3 Origin => origin;
    public Skill PrimarySkill => primarySkill;
    public bool PlayerIsActive => player;
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
        if (PlayerIsActive) ExitActivity(player);

        var unitsToRemove = new List<ActivityUnit>(Units);
        foreach (var unit in unitsToRemove)
        {
            RemoveUnit(unit);
        }

        OnActivityHasEnded?.Invoke();
        units = new List<ActivityUnit>();

        // todo: add logic to select specific unit
        // todo: add logic to make this conditional
        var targetUnit = unitsToRemove[0];
        dialogueReference.StartDialogue(targetUnit.Controller, targetUnit.Controller.Instance.InviteAFriendDialogue);
    }

    public void EnterActivity(ActivityUnit player)
    {
        this.player = player;
        AddUnit(player);
        navigation.Navigate(activityView);
        OnPlayerEnter?.Invoke(player);
    }

    public void ExitActivity(ActivityUnit player)
    {
        this.player = null;
        RemoveUnit(player);
        navigation.GoBackToRoot();
        OnPlayerExit?.Invoke(player);
    }

    public void AddUnit(ActivityUnit unit)
    {
        unit.JoinActivity(this);
        units.Add(unit);
        unit.OnXPIncreased += Unit_OnXPIncreased;
        UpdateUnits();
        OnUnitEnter?.Invoke(unit);
    }

    public void RemoveUnit(ActivityUnit unit)
    {
        unit.ExitActivity();
        units.Remove(unit);
        unit.OnXPIncreased -= Unit_OnXPIncreased;
        UpdateUnits();
        OnUnitExit?.Invoke(unit);
    }

    private void Unit_OnXPIncreased(OnXpIncreasedEventArgs args)
    {
        OnXPIncreased?.Invoke(args);
    }

    private void UpdateUnits()
    {
        int count = units.Count;
        if (count == 0) return;

        float radius = 1.0f; // radius of the circle
        float offset = Random.Range(0f, Mathf.PI * 2f); // random rotation of the circle

        for (int i = 0; i < count; i++)
        {
            // Evenly spaced angle around the circle
            float angle = -(i / (float)count) * Mathf.PI * 2f + offset;

            // Compute position on circle
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            Vector3 destination = origin + direction;

            // Update unit
            units[i].UpdatePosition(destination);
            units[i].LookAt(origin); // face the center
        }
    }
}