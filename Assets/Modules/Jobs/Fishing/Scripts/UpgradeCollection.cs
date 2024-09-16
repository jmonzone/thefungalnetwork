using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UpgradeCollection : ScriptableObject
{
    [SerializeField] private List<Upgrade> upgrades;

    public List<Upgrade> Upgrades => upgrades;

    public bool OfLevel(int level, out Upgrade upgrade)
    {
        upgrade = null;

        foreach (var _upgrade in upgrades)
        {
            if (_upgrade is NewFishUpgrade fishUpgrade && fishUpgrade.Fish.LevelRequirement == level)
            {
                upgrade = _upgrade;
                break;
            }
        }

        return upgrade;
    }

    public List<Upgrade> OfLevel(int level) => upgrades.GetRange(0, Mathf.Min(level - 1, upgrades.Count));
}
