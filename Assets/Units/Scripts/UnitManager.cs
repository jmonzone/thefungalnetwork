using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private UnitListReference unitList;
    [SerializeField] private UnitController unitPrefab;
    [SerializeField] private Transform unitSpawnAnchor;
    [SerializeField] private CameraPanController cameraPanController;

    [SerializeField] private List<UnitController> unitControllers;

    [Header("Initial Party Frog")]
    [SerializeField] private Transform frogSpawnAnchor;

    public List<UnitController> UnitControllers => unitControllers;
    public List<UnitController> AllUnits => unitControllers.Concat(partyReference.Guests).ToList();

    public event UnityAction<UnitController> OnUnitSummoned;

    private void Awake()
    {
        unitControllers = new List<UnitController>();
    }

    private void Start()
    {
        //SummonPartyFrog();
        foreach(var unit in unitList.Units)
        {
            SummonUnit(unit);
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

    private void SummonUnit(Unit unit)
    {
        var spawnPosition = unitSpawnAnchor.transform.position;
        var randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0;
        spawnPosition += randomDirection * 2f;

        var unitController = Instantiate(unitPrefab, spawnPosition, unitSpawnAnchor.transform.rotation);
        unitController.Initialize(unit);
        unitControllers.Add(unitController);

        //cameraPanController.CenterTargetInView(unitController.transform.position);
        OnUnitSummoned?.Invoke(unitController);
    }

    private void SummonPartyFrog()
    {
        var spawnPosition = frogSpawnAnchor.transform.position;

        var unitController = Instantiate(unitPrefab, spawnPosition, frogSpawnAnchor.transform.rotation);
        unitController.Initialize(unitList.Units[0]);
        unitControllers.Add(unitController);

        //cameraPanController.CenterTargetInView(unitController.transform.position);
        OnUnitSummoned?.Invoke(unitController);
    }
}
