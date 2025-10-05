using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class DanceActivityUI : ActivityUI<UnitDance, DanceActivityController>
{
    [Header("References")]
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private DJTableReference djTableReference;
    [SerializeField] private DanceActivityController danceActivity;
    [SerializeField] private DanceBackground background;
    [SerializeField] private DanceMoveUIManager danceMoveUIManager;

    protected override Camera Camera => background.DominantCamera;

    protected override void Awake()
    {
        base.Awake();
        danceMoveUIManager.Initialize();
    }

    protected override void OnPlayerEnter(ActivityUnit player)
    {
        base.OnPlayerEnter(player);
        UpdateMovesUI();

        background.StartDanceBackground();
    }

    protected override void OnPlayerExit(ActivityUnit player)
    {
        base.OnPlayerExit(player);
        background.EndDanceBackground();
        StopAllCoroutines();
    }

    protected override void OnUnitSelected(UnitDance unit)
    {
        base.OnUnitSelected(unit);
        UpdateMovesUI();
    }

    private void UpdateMovesUI()
    {
        if (danceActivity.CurrentUnit)
        {
            var moves = danceActivity.CurrentUnit.Instance.Skills[Activity.PrimarySkill].Moves;
            StartCoroutine(danceMoveUIManager.Show(danceActivity.CurrentUnit, moves, () =>
            {
                SetExitButtonInteractable(false);
            },
            () =>
            {
                SetExitButtonInteractable(true);
            }));
        }
    }

    protected override IEnumerator LevelUI_OnExitRoutine()
    {
        UpdateMovesUI();
        yield return base.LevelUI_OnExitRoutine();
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
