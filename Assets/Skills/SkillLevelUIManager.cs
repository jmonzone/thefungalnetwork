using System.Collections.Generic;
using UnityEngine;

public class SkillLevelUIManager : MonoBehaviour
{
    [SerializeField] private Skill skill;
    private List<SkillLevelUI> skillLevelUIViews;

    public Dictionary<UnitInstance, SkillLevelUI> UnitLevelViewMap { get; private set; } = new Dictionary<UnitInstance, SkillLevelUI>();

    private void Awake()
    {
        skillLevelUIViews = new List<SkillLevelUI>();
        GetComponentsInChildren(true, skillLevelUIViews);
    }

    public void SetUnits(IEnumerable<UnitInstance> units)
    {

        skillLevelUIViews = new List<SkillLevelUI>();
        GetComponentsInChildren(true, skillLevelUIViews);
        UnitLevelViewMap = new Dictionary<UnitInstance, SkillLevelUI>();

        var i = 0;
        foreach(var unit in units)
        {
            skillLevelUIViews[i].SetUnit(unit, unit.Skills[skill]);
            skillLevelUIViews[i].gameObject.SetActive(true);
            UnitLevelViewMap.Add(unit, skillLevelUIViews[i]);
            i++;
        }

        while (i < skillLevelUIViews.Count)
        {
            skillLevelUIViews[i].gameObject.SetActive(false);
            i++;
        }
    }

}
