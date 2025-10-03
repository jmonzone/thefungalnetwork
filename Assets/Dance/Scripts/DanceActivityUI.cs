using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DanceActivityUI : ActivityUI
{
    [Header("References")]
    [SerializeField] private DanceActivityController danceActivity;
    [SerializeField] private DanceBackground background;
    [SerializeField] private DanceMoveUIManager danceMoveUIManager;
    [SerializeField] private Image touchIndicator;
    [SerializeField] private FadeCanvasGroup gameplayUI;
    [SerializeField] private LevelUpUI levelUpUI;

    [Header("Settings")]
    [SerializeField] private float touchDuration = 0.2f;
    [SerializeField] private float touchScale = 1.5f;

    protected override Camera Camera => background.DominantCamera;
    private bool canSelect = false;

    protected override void Awake()
    {
        base.Awake();
        levelUpUI.gameObject.SetActive(false);
        levelUpUI.OnExit += () => StartCoroutine(LevelUI_OnExitRoutine());
        danceMoveUIManager.Initialize();
    }

    private IEnumerator LevelUI_OnExitRoutine()
    {
        yield return levelUpUI.Hide();
        yield return gameplayUI.FadeIn();
    }

    protected override void OnPlayerEnter(PlayerController player)
    {
        base.OnPlayerEnter(player);
        danceActivity.OnUnitSelected += OnUnitSelected;
        OnUnitSelected();


        StartCoroutine(EnterRoutine());
    }

    private IEnumerator EnterRoutine()
    {
        background.StartDanceBackground();
        yield return gameplayUI.FadeIn();
        canSelect = true;

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShowTouchIndicator(Input.mousePosition);

                //if (canSelect && TryRaycastUnit(out UnitController unit) && Activity.Units.Contains(unit))
                //{
                //    danceActivity.SelectUnit(unit);
                //}
            }

            yield return null;
        }
    }

    protected override void OnPlayerExit(PlayerController player)
    {
        base.OnPlayerExit(player);
        danceActivity.OnUnitSelected -= OnUnitSelected;
        background.EndDanceBackground();
        StopAllCoroutines();
    }

    private void OnUnitSelected()
    {
        if (danceActivity.SelectedUnit)
        {
            var moves = danceActivity.SelectedUnit.Instance.Skills[Activity.PrimarySkill].Moves;
            StartCoroutine(danceMoveUIManager.Show(danceActivity.SelectedUnit, moves, () =>
            {
                canSelect = false;
                SetExitButtonInteractable(false);
            },
            () =>
            {
                canSelect = true;
                SetExitButtonInteractable(true);
            }));
        }
    }

    private void Instance_OnMoveUnlocked(UnitInstance unit, DanceMoveInstance move, bool isUpgrade)
    {
        Debug.Log($"{unit.name} unlocked {move.name}");
        //StartCoroutine(LevelUpRoutine(unit, move, isUpgrade));
        //UpdateDanceMovesUI();
    }

    //private IEnumerator LevelUpRoutine(UnitInstance unit, DanceMoveInstance move, bool isUpgrade)
    //{
    //    yield return gameplayUI.FadeOut();
    //    yield return levelUpUI.Show(unit, unit.Skills[PrimarySkill], move);
    //}

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

    private Coroutine touchRoutine;
    private void ShowTouchIndicator(Vector3 screenPos)
    {
        if (!touchIndicator) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            touchIndicator.transform.parent as RectTransform,
            screenPos,
            null,
            out Vector2 localPos
        );

        touchIndicator.rectTransform.anchoredPosition = localPos;

        if (touchRoutine != null) StopCoroutine(touchRoutine);
        touchRoutine = StartCoroutine(TouchIndicatorRoutine());
    }

    private IEnumerator TouchIndicatorRoutine()
    {
        touchIndicator.gameObject.SetActive(true);
        touchIndicator.transform.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < touchDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / touchDuration;
            touchIndicator.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * touchScale, t);
            //touchIndicator.color = Color.Lerp(djReference.DominantTrack.Glyph.Color, Color.clear, t);
            yield return null;
        }

        touchIndicator.gameObject.SetActive(false);
    }
}
