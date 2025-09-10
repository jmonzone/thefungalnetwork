using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UnitManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private UnitListReference unitList;
    [SerializeField] private UnitController unitPrefab;
    [SerializeField] private Transform unitSpawnAnchor;

    [Header("Runtime")]
    [SerializeField] private List<UnitController> unitControllers;

    public List<UnitController> UnitControllers => unitControllers;
    public List<UnitController> AllUnits => unitControllers.Concat(partyReference.Guests).ToList();

    public event UnityAction<UnitController> OnUnitSummoned;

    private void Awake()
    {
        unitControllers = new List<UnitController>();
    }

    private void Start()
    {
        foreach(var unit in unitList.Units)
        {
            if (unit.IsFriends) SummonUnit(unit);
        }
    }

    private void OnEnable()
    {
        unitList.OnUnitSummoned += SummonUnit;
    }

    private void OnDisable()
    {
        unitList.OnUnitSummoned -= SummonUnit;
    }

    private void SummonUnit(UnitInstance unit)
    {
        var spawnPosition = unitSpawnAnchor.transform.position;
        var randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0;
        spawnPosition += randomDirection * 2f;

        var unitController = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
        unitController.Initialize(unit);
        unitControllers.Add(unitController);

        OnUnitSummoned?.Invoke(unitController);
    }
}
