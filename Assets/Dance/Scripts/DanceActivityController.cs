using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DanceActivityController : ActivityController
{
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private Light spotlight;

    private int selectedIndex;
    private UnitController selectedUnit;

    public UnitController SelectedUnit => selectedUnit;

    public event UnityAction OnUnitSelected;

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

            if (selectedUnit)
            {
                spotlight.transform.position = selectedUnit.transform.position + Vector3.up * 5f;
            }

            yield return null;
        }
    }

    protected override void OnActivityEnded()
    {
        base.OnActivityEnded();
        StopAllCoroutines();
    }

    protected override void OnUnitEnter(UnitController unit)
    {
        base.OnUnitEnter(unit);
        var dancer = unit.GetComponent<UnitDance>();
        dancer.OnDanceMoveUsed += OnDanceMoveUsed;
        dancer.OnDanceMoveComplete += OnDanceMoveComplete;
        unit.SetBehaviour(dancer);
    }

    protected override void OnUnitExit(UnitController unit)
    {
        base.OnUnitExit(unit);
        var dancer = unit.GetComponent<UnitDance>();
        unit.ApplyDefaultBehaviour();
        dancer.OnDanceMoveUsed -= OnDanceMoveUsed;
        dancer.OnDanceMoveComplete -= OnDanceMoveComplete;
    }

    private void OnDanceMoveUsed(UnitController unit, DanceMoveInstance danceMove)
    {
        Activity.IncreaseXP(unit, danceMove.Xp);
    }

    private void OnDanceMoveComplete(UnitController unit, DanceMoveInstance danceMove)
    {
        selectedIndex = (selectedIndex + 1) % Activity.Units.Count;
        SelectUnit(Activity.Units[selectedIndex]);
    }

    protected override void OnPlayerEnter(PlayerController player)
    {
        base.OnPlayerEnter(player);
        selectedIndex = Activity.Units.FindIndex(unit => unit == player);
        SelectUnit(player);
    }

    protected override void OnPlayerExit(PlayerController player)
    {
        base.OnPlayerExit(player);
        if (player == selectedUnit) UnselectUnit();
    }

    public void SelectUnit(UnitController unit)
    {
        if (selectedUnit == unit) return;

        Debug.Log($"SelectUnit {unit.name}");
        UnselectUnit();

        selectedUnit = unit;
        selectedUnit.GetComponent<UnitDance>().Highlight();

        spotlight.gameObject.SetActive(true);
        OnUnitSelected?.Invoke();
    }

    private void AutoSelectDanceMove()
    {
        if (!PlayerIsActive)
        {
            var moves = selectedUnit.Instance.Skills[Activity.PrimarySkill].Moves;
            var randomMove = moves[Random.Range(0, moves.Count)];
            selectedUnit.GetComponent<UnitDance>().UseDanceMove(randomMove, () => AutoSelectDanceMove());
        }
    }

    private void UnselectUnit()
    {
        if (selectedUnit)
        {
            selectedUnit.GetComponent<UnitDance>().Unhighlight();
            spotlight.gameObject.SetActive(false);

            selectedUnit = null;
        }
    }
}
