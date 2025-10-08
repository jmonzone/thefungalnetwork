using System.Collections;
using UnityEngine;

public class DanceActivityController : ActivityController<DanceActivityUnit>
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

    protected override void OnUnitBehaviourApplied(DanceActivityUnit unit)
    {
        base.OnUnitBehaviourApplied(unit);
        unit.OnDanceMoveUsed += OnDanceMoveUsed;
    }

    protected override void OnUnitBehaviourRemoved(DanceActivityUnit unit)
    {
        base.OnUnitBehaviourRemoved(unit);
        unit.OnDanceMoveUsed -= OnDanceMoveUsed;
    }

    private void OnDanceMoveUsed(DanceActivityUnit unit, DanceMoveInstance danceMove)
    {
        unit.IncreaseXP(danceMove.Xp, unit.transform.position + Vector3.up * 0.5f);
    }

    public override void SelectUnit(DanceActivityUnit unit)
    {
        base.SelectUnit(unit);
        spotlight.gameObject.SetActive(true);
        if (!unit.IsPlayer) StartCoroutine(PassRoutine());
    }       

    private IEnumerator PassRoutine()
    {
        yield return new WaitUntil(() => CurrentUnit.Controller.Destination.IsAtDestination);
        yield return new WaitForSeconds(djReference.BeatDuration * 2);

        var moves = CurrentUnit.Instance.Skills[Activity.PrimarySkill].Moves;
        var randomMove = moves[Random.Range(0, moves.Count)];
        CurrentUnit.UseDanceMove(randomMove, () => SelectNextUnit());
    }


    protected override void UnselectUnit()
    {
        base.UnselectUnit();

        if (CurrentUnit)
        {
            spotlight.gameObject.SetActive(false);
        }
    }
}
