using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class UpgradeCollection : ScriptableObject
{
    [SerializeField] private List<Upgrade> upgrades;

    public List<Upgrade> Upgrades => upgrades;

    public bool OfLevel(int level, out Upgrade upgrade)
    {
        upgrade = upgrades.ElementAtOrDefault(level - 2);
        if (upgrade) Debug.Log($"{level} {upgrade.name}");
        else Debug.Log($"{level}");
        return upgrade;
    }

    public List<Upgrade> OfLevel(int level) => upgrades.GetRange(0, Mathf.Min(level - 1, upgrades.Count));
}
