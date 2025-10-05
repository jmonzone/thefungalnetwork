using System.Collections;
using UnityEngine;

public class DanceActivityController : ActivityController<UnitDance>
{
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private Light spotlight;
    [SerializeField] private UnitManager unitManager;

    protected override void OnActivityStart()
    {
        base.OnActivityStart();
        StartCoroutine(ActivityRoutine());
    }

    private IEnumerator ActivityRoutine()
    {
        //var timer = 0f;
        while (true)
        {
            //timer += Time.deltaTime;

            //if (timer > djReference.BeatDuration * 2f)
            //{
            //    foreach (var unit in Activity.Units)
            //    {
            //        Activity.IncreaseXP(unit, 1f);
            //    }

            //    timer = 0;
            //}

            if (CurrentUnit)
            {
                spotlight.transform.position = CurrentUnit.transform.position + Vector3.up * 5f;
            }

            yield return null;
        }
    }

    protected override void OnActivityEnded()
    {
        base.OnActivityEnded();
        StopAllCoroutines();
    }

    protected override void OnUnitBehaviourApplied(UnitDance unit)
    {
        base.OnUnitBehaviourApplied(unit);
        unit.OnDanceMoveUsed += OnDanceMoveUsed;
        unit.OnDanceMoveComplete += OnDanceMoveComplete;
    }

    protected override void OnUnitBehaviourRemoved(UnitDance unit)
    {
        base.OnUnitBehaviourRemoved(unit);
        unit.OnDanceMoveUsed -= OnDanceMoveUsed;
        unit.OnDanceMoveComplete -= OnDanceMoveComplete;
    }

    private void OnDanceMoveUsed(UnitDance unit, DanceMoveInstance danceMove)
    {
        unit.IncreaseXP(danceMove.Xp, unit.transform.position + Vector3.up * 0.5f);
    }

    private void OnDanceMoveComplete(UnitDance unit, DanceMoveInstance danceMove)
    {
        SelectNextUnit();
    }

    private void AutoSelectDanceMove()
    {
        if (!PlayerIsActive)
        {
            var moves = CurrentUnit.Instance.Skills[Activity.PrimarySkill].Moves;
            var randomMove = moves[Random.Range(0, moves.Count)];
            CurrentUnit.UseDanceMove(randomMove, () => AutoSelectDanceMove());
        }
    }

    public override void SelectUnit(UnitDance unit)
    {
        base.SelectUnit(unit);
        unit.Highlight();
        spotlight.gameObject.SetActive(true);
    }

    protected override void UnselectUnit()
    {
        base.UnselectUnit();

        if (CurrentUnit)
        {
            CurrentUnit.Unhighlight();
            spotlight.gameObject.SetActive(false);
        }
    }
}
