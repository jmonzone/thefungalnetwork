using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private UnitListReference unitList;

    [SerializeField] private List<UnitController> unitControllers;
    [SerializeField] private UnitController initialUnit;
    [SerializeField] private UnitController unitPrefab;

    [SerializeField] private Transform unitSpawnAnchor;
    [SerializeField] private CameraPanController cameraPanController;

    private void Awake()
    {
        unitControllers = new List<UnitController> { initialUnit };
    }

    private void OnEnable()
    {
        unitList.OnUnitSummoned += UnitList_OnUnitSummoned;
    }

    private void OnDisable()
    {
        unitList.OnUnitSummoned -= UnitList_OnUnitSummoned;
    }

    private void UnitList_OnUnitSummoned(Unit unit)
    {
        var unitController = Instantiate(unitPrefab, unitSpawnAnchor.transform.position, unitSpawnAnchor.transform.rotation);
        unitController.Initialize(unit);
        unitControllers.Add(unitController);

        cameraPanController.CenterTargetInView(unitController.transform.position);

    }
}
