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

    [Header("Settings")]
    [SerializeField] private float touchDuration = 0.2f;
    [SerializeField] private float touchScale = 1.5f;

    protected override Camera Camera => background.DominantCamera;

    protected override void Awake()
    {
        base.Awake();
        danceMoveUIManager.Initialize();
    }

    protected override void OnPlayerEnter(PlayerController player)
    {
        base.OnPlayerEnter(player);
        danceActivity.OnUnitSelected += UpdateMovesUI;
        UpdateMovesUI();

        StartCoroutine(EnterRoutine());
    }

    private IEnumerator EnterRoutine()
    {
        background.StartDanceBackground();

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
        danceActivity.OnUnitSelected -= UpdateMovesUI;
        background.EndDanceBackground();
        StopAllCoroutines();
    }

    private void UpdateMovesUI()
    {
        if (danceActivity.SelectedUnit)
        {
            var moves = danceActivity.SelectedUnit.Instance.Skills[Activity.PrimarySkill].Moves;
            StartCoroutine(danceMoveUIManager.Show(danceActivity.SelectedUnit, moves, () =>
            {
                SetExitButtonInteractable(false);
            },
            () =>
            {
                SetExitButtonInteractable(true);
            }));
        }
    }

    protected override IEnumerator LevelUpRoutine(UnitInstance unit)
    {
        yield return base.LevelUpRoutine(unit);
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
