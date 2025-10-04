using System.Collections.Generic;
using UnityEngine;

public class SkillLevelUIManager : MonoBehaviour
{
    [SerializeField] private Skill skill;
    private List<SkillLevelUI> skillLevelUIViews;

    public Dictionary<ActivityUnit, SkillLevelUI> UnitLevelViewMap { get; private set; } = new Dictionary<ActivityUnit, SkillLevelUI>();

    public void Show(IEnumerable<ActivityUnit> units)
    {
        gameObject.SetActive(true);

        skillLevelUIViews = new List<SkillLevelUI>();
        GetComponentsInChildren(true, skillLevelUIViews);

        UnitLevelViewMap = new Dictionary<ActivityUnit, SkillLevelUI>();

        var i = 0;
        foreach (var unit in units)
        {
            skillLevelUIViews[i].SetUnit(unit);
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
