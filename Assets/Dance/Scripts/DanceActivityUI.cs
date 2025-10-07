using System.Collections;
using UnityEngine;

public class DanceActivityUI : ActivityUI<DanceActivityUnit, DanceActivityController>
{
    [Header("References")]
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private DJTableReference djTableReference;
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

    protected override void OnUnitSelected(DanceActivityUnit unit)
    {
        base.OnUnitSelected(unit);
        UpdateMovesUI();
    }

    private void UpdateMovesUI()
    {
        if (Controller.CurrentUnit)
        {
            if (Controller.CurrentUnit.IsUsingDanceMove)
            {
                danceMoveUIManager.ToggleInteractable(false);
            }
            else
            {
                danceMoveUIManager.ToggleInteractable(Controller.CurrentUnit.IsPlayer);
            }

            if (Controller.CurrentUnit.IsPlayer)
            {
                var moves = Controller.CurrentUnit.Instance.Skills[Activity.PrimarySkill].Moves;
                StartCoroutine(danceMoveUIManager.Show(Controller.CurrentUnit, moves, () =>
                {
                    danceMoveUIManager.ToggleInteractable(false);
                    SetExitButtonInteractable(false);
                },
                () =>
                {
                    Controller.SelectNextUnit();
                    SetExitButtonInteractable(true);
                }));
            }
            else
            {
                danceMoveUIManager.Hide();
            }
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
