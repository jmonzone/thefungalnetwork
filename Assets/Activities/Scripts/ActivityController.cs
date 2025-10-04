using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ActivityController<T> : MonoBehaviour where T : ActivityBehaviour
{
    [Header("Activity References")]
    [SerializeField] private ActivityReference activity;
    [SerializeField] private Skill primarySkill;

    [Header("Activity Runtime")]
    [SerializeField] private bool playerIsActive;
    [SerializeField] private bool playerIsSelected;
    [SerializeField] private int selectedIndex;
    [SerializeField] private T selectedUnit;
    [SerializeField] private List<T> units;

    protected ActivityReference Activity => activity;
    protected Skill PrimarySkill => primarySkill;
    protected bool PlayerIsActive => playerIsActive;
    protected bool PlayerIsSelected => playerIsActive;
    public T SelectedUnit => selectedUnit;
    protected List<T> Units => units;

    public event UnityAction OnUnitWasSelected;

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

    protected void OnPlayerEnter(ActivityUnit player)
    {
        playerIsActive = true;
        selectedIndex = units.FindIndex(unit => unit == player);
        SelectUnit(player.GetComponent<T>());
    }

    protected void OnPlayerExit(ActivityUnit player)
    {
        playerIsActive = false;
        if (playerIsSelected) UnselectUnit();
    }

    protected virtual void OnActivityStart()
    {
    }

    protected virtual void OnActivityEnded()
    {
    }

    protected void OnUnitEnter(ActivityUnit unit)
    {
        var activityBehaviour = unit.GetComponent<T>();
        unit.SetBehaviour(activityBehaviour);
        OnUnitBehaviourApplied(activityBehaviour);
    }

    protected void OnUnitExit(ActivityUnit unit)
    {
        var activityBehaviour = unit.GetComponent<T>();
        OnUnitBehaviourRemoved(activityBehaviour);
    }

    protected virtual void OnUnitBehaviourApplied(T unit)
    {
        units.Add(unit);
    }


    protected virtual void OnUnitBehaviourRemoved(T unit)
    {
        units.Remove(unit);
    }

    public void SelectUnit(T unit)
    {
        if (selectedUnit == unit) return;

        Debug.Log($"SelectUnit {unit.name}");
        UnselectUnit();

        selectedUnit = unit;
        playerIsSelected = unit.Controller is PlayerController;
        OnUnitSelected(selectedUnit);
        OnUnitWasSelected?.Invoke();
    }


    protected void UnselectUnit()
    {
        if (selectedUnit)
        {
            if (playerIsSelected && selectedUnit.Controller is PlayerController)
            {
                playerIsSelected = false;
            }

            OnUnitUnselected(selectedUnit);

            selectedUnit = null;
        }
    }

    protected virtual void OnUnitSelected(T unit)
    {

    }

    protected virtual void OnUnitUnselected(T unit)
    {

    }

    protected void SelectNextUnit()
    {
        selectedIndex = (selectedIndex + 1) % Activity.Units.Count;
        SelectUnit(units[selectedIndex]);
    }
}
