using System.Collections;
using UnityEngine;

public class DancefloorGameplayUI : ActivityController
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private DancefloorBackground background;
    [SerializeField] private Skill danceSkill;

    protected override Camera Camera => background.DominantCamera;

    protected override IEnumerator OnActivityStart()
    {
        foreach (var unit in Activity.Units)
        {
            var dance = unit.GetComponent<UnitDance>();
            unit.SetBehaviour(dance);
            LevelUI.UnitLevelViewMap[unit.Instance].SetColor(unit.Color);
        }

        var timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;

            if (timer > djReference.BeatDuration)
            {
                foreach (var unit in Activity.Units)
                {
                    IncreaseXP(unit, 1f);
                }

                timer = 0;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Activity.Units[0].GetComponent<UnitDance>().IncrementDancePower();
                IncreaseXP(Activity.Units[0], 1f);
            }

            yield return null;
        }
    }

    protected override void OnActivityEnded()
    {
        base.OnActivityEnded();
        StopAllCoroutines();
    }
}
