using System.Collections.Generic;
using UnityEngine;

public class UnitListUI : MonoBehaviour
{
    [SerializeField] private UnitListReference unitListReference;

    private List<UnitUI> unitViewList = new List<UnitUI>();

    private void Awake()
    {
        GetComponentsInChildren(true, unitViewList);
    }

    private void OnEnable()
    {
        UpdateView();
        unitListReference.OnShow += UpdateView;
        unitListReference.OnUnitSummoned += UnitListReference_OnUnitSummoned;
    }

    private void UnitListReference_OnUnitSummoned(Unit arg0)
    {
        UpdateView();
    }

    private void OnDisable()
    {
        unitListReference.OnShow -= UpdateView;
        unitListReference.OnUnitSummoned -= UnitListReference_OnUnitSummoned;
    }

    private void UpdateView()
    {
        var i = 0;
        foreach(var unitView in unitViewList)
        {
            if (i < unitListReference.Units.Count)
            {
                var unit = unitListReference.Units[i];
                unitView.SetUnit(unit);
            }
            else
            {
                unitView.SetUnit(null);
            }

            unitView.gameObject.SetActive(i <= unitListReference.Units.Count);

            i++;
        }
    }
}
