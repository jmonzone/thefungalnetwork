using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class UnitListReference : UIReference
{
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private List<Unit> units;

    [SerializeField] private Unit initialUnit;
    [SerializeField] private Unit summonedUnit;

    public List<Unit> Units => units;

    public event UnityAction OnUnitsUpdated;

    public void Initialize()
    {
        units = new List<Unit> { initialUnit };
    }

    public void Summon()
    {
        if (inventory.SporeCount >= 125)
        {
            inventory.DecreaseSporeCount(125);
            units.Add(summonedUnit);
            OnUnitsUpdated?.Invoke();
        }
    }
}
