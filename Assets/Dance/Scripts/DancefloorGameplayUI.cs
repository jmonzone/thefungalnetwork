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

            if (timer > djReference.BeatDuration * 2f)
            {
                foreach (var unit in Activity.Units)
                {
                    IncreaseXP(unit, 1f);
                }

                timer = 0;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (TryRaycastUnit(out UnitController unit))
                {
                    if (Activity.Units.Contains(unit))
                    {
                        unit.GetComponent<UnitDance>().IncrementDancePower();
                        IncreaseXP(unit, 1f);
                    }
                }
            }

            yield return null;
        }
    }

    protected override void OnActivityEnded()
    {
        base.OnActivityEnded();
        StopAllCoroutines();
    }

    private bool TryRaycastUnit(out UnitController unit)
    {
        var ray = Camera.ScreenPointToRay(Input.mousePosition);

        var raycastHits = Physics.RaycastAll(ray);

        foreach(var hit in raycastHits)
        {
            unit = hit.transform.GetComponentInParent<UnitController>();
            if (unit) return unit;
        }

        unit = null;
        return false;
    }
}
