using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLevelUIManager : MonoBehaviour
{
    private List<SkillLevelUI> skillLevelUIViews;

    public Dictionary<UnitInstance, SkillLevelUI> UnitLevelViewMap { get; private set; } = new Dictionary<UnitInstance, SkillLevelUI>();

    private void Awake()
    {
        skillLevelUIViews = new List<SkillLevelUI>();
        GetComponentsInChildren(true, skillLevelUIViews);

        UnitLevelViewMap = new Dictionary<UnitInstance, SkillLevelUI>();
    }

    public void SetUnits(IEnumerable<UnitInstance> units)
    {
        var i = 0;
        foreach(var unit in units)
        {
            skillLevelUIViews[i].SetUnit(unit);
            UnitLevelViewMap.Add(unit, skillLevelUIViews[i]);
            i++;
        }
    }

}
