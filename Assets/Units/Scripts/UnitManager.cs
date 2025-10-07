using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UnitManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnitListReference unitList;
    [SerializeField] private UnitController unitPrefab;
    [SerializeField] private Transform unitSpawnAnchor;
    [SerializeField] private PlayerController playerController;

    [Header("Runtime")]
    [SerializeField] private List<UnitController> unitControllers;

    public List<UnitController> UnitControllers => unitControllers;

    public event UnityAction<UnitController> OnUnitSummoned;
    public event UnityAction OnAllUnitsSummoned;

    private void Awake()
    {
        unitControllers = new List<UnitController>();
    }

    private void Start()
    {
        playerController.Initialize(unitList.Units[0]);

        foreach (var unit in unitList.Units)
        {
            if (unit == playerController.Instance) continue;
            SummonUnit(unit);
        }

        OnAllUnitsSummoned?.Invoke();
    }

    private void OnEnable()
    {
        unitList.OnFriendInvited += UnitList_OnFriendInvited;
    }

    private void OnDisable()
    {
        unitList.OnFriendInvited -= UnitList_OnFriendInvited;
    }

    private void UnitList_OnFriendInvited(UnitInstance unit)
    {
        SummonUnit(unit);
    }

    public UnitController SummonUnit(UnitInstance unit)
    {
        var spawnPosition = unitSpawnAnchor.transform.position;
        var randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0;
        spawnPosition += randomDirection * 2f;

        var unitController = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
        unitController.Initialize(unit);
        unitControllers.Add(unitController);

        OnUnitSummoned?.Invoke(unitController);
        return unitController;
    }
}
