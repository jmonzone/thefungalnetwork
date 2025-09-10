using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitListUI : MonoBehaviour
{
    [SerializeField] private UnitListReference unitListReference;
    [SerializeField] private Transform unitViewContainer;
    [SerializeField] private UnitUI unitViewPrefab;

    private List<UnitUI> unitViewList = new List<UnitUI>();

    private void Awake()
    {
        GetComponentsInChildren(true, unitViewList);
    }

    private void OnEnable()
    {
        UpdateView();
        unitListReference.OnFungalOpened += UpdateView;
        unitListReference.OnUnitSummoned += UnitListReference_OnUnitSummoned;
    }

    private void UnitListReference_OnUnitSummoned(UnitInstance arg0)
    {
        UpdateView();
    }

    private void OnDisable()
    {
        unitListReference.OnFungalOpened -= UpdateView;
        unitListReference.OnUnitSummoned -= UnitListReference_OnUnitSummoned;
    }

    private void UpdateView()
    {
        // filter only hired units
        var hiredUnits = unitListReference.Units.Where(u => u.IsHired).ToList();

        // ensure we have enough views
        while (unitViewList.Count < hiredUnits.Count)
        {
            var newView = Instantiate(unitViewPrefab, unitViewContainer);
            unitViewList.Add(newView);
        }

        for (int i = 0; i < unitViewList.Count; i++)
        {
            if (i < hiredUnits.Count)
            {
                unitViewList[i].SetUnit(hiredUnits[i]);
                unitViewList[i].gameObject.SetActive(true);
            }
            else
            {
                unitViewList[i].SetUnit(null);
                unitViewList[i].gameObject.SetActive(false);
            }
        }
    }
}
