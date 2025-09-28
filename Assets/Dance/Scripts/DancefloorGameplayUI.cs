using System.Collections;
using UnityEngine;

public class DancefloorGameplayUI : ActivityController
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private DancefloorBackground background;

    protected override IEnumerator OnActivityStart()
    {
        foreach (var unit in Activity.Units)
        {
            var dance = unit.GetComponent<UnitDance>();
            unit.SetBehaviour(dance);
            LevelUI.UnitLevelViewMap[unit.Instance].SetColor(dance.CurrentColor);
        }

        var timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;

            if (timer > djReference.BeatDuration)
            {
                foreach (var unit in Activity.Units)
                {
                    IncreaseDanceXP(unit, 1f);
                }

                timer = 0;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Activity.Units[0].GetComponent<UnitDance>().IncrementDancePower();
                IncreaseDanceXP(Activity.Units[0], 1f);
            }

            yield return null;
        }
    }

    private void IncreaseDanceXP(UnitController unit, float value)
    {
        unit.Instance.IncreaseSkillXP(Skill.DANCE, 1f);


        var dancer = unit.GetComponent<UnitDance>();

        var worldPos = unit.transform.position + Vector3.up;
        Vector3 viewportPos = background.DominantCamera.WorldToScreenPoint(worldPos);
        LevelUI.UnitLevelViewMap[unit.Instance].SetColor(dancer.CurrentColor);
        LevelUI.UnitLevelViewMap[unit.Instance].Increase(value, viewportPos);
    }
}
