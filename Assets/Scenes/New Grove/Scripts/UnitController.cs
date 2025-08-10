using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitController : MonoBehaviour
{
    [SerializeField] private bool isSelected = false;

    public event UnityAction OnSelected;

    public void Select()
    {
        isSelected = true;
        OnSelected?.Invoke();
    }
}
