using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitController : MonoBehaviour
{
    [SerializeField] private Unit data;
    [SerializeField] private bool isSelected = false;

    public Unit Data => data;

    public event UnityAction OnInitialized;
    public event UnityAction OnSelected;
    public event UnityAction OnUnselect;

    public void Initialize(Unit data)
    {
        this.data = data;
        name = "Unit Controller - " + data.name; 
        Instantiate(data.Prefab, transform);
        OnInitialized?.Invoke();
    }

    public void Select()
    {
        isSelected = true;
        OnSelected?.Invoke();
    }

    public void Unselect()
    {
        isSelected = false;
        OnUnselect?.Invoke();
    }
}
