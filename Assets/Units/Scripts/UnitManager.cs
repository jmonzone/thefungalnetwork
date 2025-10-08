using System.Collections;
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
            SummonUnit(unit, GetRandomSpawnPosition());
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
        StartCoroutine(SpawnRoutine(unit));
    }

    protected UnitController SummonUnit(UnitInstance unit, Vector3 spawnPosition)
    {
        var unitController = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
        unitController.Initialize(unit);
        unitControllers.Add(unitController);

        OnUnitSummoned?.Invoke(unitController);
        return unitController;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        var spawnPosition = unitSpawnAnchor.transform.position;
        var randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0;
        spawnPosition += randomDirection * 2f;
        return spawnPosition;
    }

    [SerializeField] private PortalController portalPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnOffsetY = -1f; // how deep the Fungal starts below ground
    [SerializeField] private AnimationCurve riseCurve;
    [SerializeField] private float riseDuration = 1f;

    private IEnumerator SpawnRoutine(UnitInstance unit)
    {
        var spawnPosition = GetRandomSpawnPosition();

        // Step 1: create portal
        var portal = Instantiate(portalPrefab, spawnPosition, Quaternion.identity);

        bool portalOpened = false;
        portal.OnOpened.AddListener(() => portalOpened = true);

        // Wait until the portal finishes opening
        yield return new WaitUntil(() => portalOpened);

        // Step 2: spawn fungal
        var fungal = SummonUnit(unit, spawnPosition + Vector3.down);

        // Animate the fungal rising out of the portal
        float elapsed = 0f;
        Vector3 start = fungal.transform.position;
        Vector3 end = spawnPosition;

        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = riseCurve.Evaluate(elapsed / riseDuration);
            fungal.transform.position = Vector3.LerpUnclamped(start, end, t);
            yield return null;
        }

        fungal.transform.position = end;
    }
}
