using System.Collections.Generic;
using UnityEngine;

public class SkillLevelUIManager : MonoBehaviour
{
    [SerializeField] private Skill skill;
    private List<SkillLevelUI> skillLevelUIViews;

    public Dictionary<UnitController, SkillLevelUI> UnitLevelViewMap { get; private set; } = new Dictionary<UnitController, SkillLevelUI>();

    public void Show(IEnumerable<UnitController> units)
    {
        gameObject.SetActive(true);

        skillLevelUIViews = new List<SkillLevelUI>();
        GetComponentsInChildren(true, skillLevelUIViews);

        UnitLevelViewMap = new Dictionary<UnitController, SkillLevelUI>();

        var i = 0;
        foreach (var unit in units)
        {
            skillLevelUIViews[i].SetUnit(unit, unit.Instance.Skills[skill]);
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
