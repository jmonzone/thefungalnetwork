using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitController : MonoBehaviour
{
    [SerializeField] private bool isSelected = false;

    public event UnityAction OnInitialized;
    public event UnityAction OnSelected;

    public void Initialize(Unit unit)
    {
        name = "Unit Controller - " + unit.name; 
        Instantiate(unit.Prefab, transform);
        OnInitialized?.Invoke();
    }

    public void Select()
    {
        isSelected = true;
        OnSelected?.Invoke();
    }
}
