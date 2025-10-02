using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DanceActivityUI : ActivityController
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private DanceBackground background;
    [SerializeField] private DanceMoveUIManager danceMoveUIManager;
    [SerializeField] private Image touchIndicator;
    [SerializeField] private Light spotlight;
    [SerializeField] private FadeCanvasGroup gameplayCanvas;
    [SerializeField] private LevelUpUI levelUpUI;

    [Header("Settings")]
    [SerializeField] private float touchDuration = 0.2f;
    [SerializeField] private float touchScale = 1.5f;

    protected override Camera Camera => background.DominantCamera;

    protected override void Awake()
    {
        base.Awake();
        levelUpUI.gameObject.SetActive(false);
        levelUpUI.OnExit += () => StartCoroutine(LevelUI_OnExitRoutine());
    }

    private IEnumerator LevelUI_OnExitRoutine()
    {
        yield return levelUpUI.Hide();
        yield return gameplayCanvas.FadeIn();
    }

    protected override IEnumerator OnActivityStart()
    {
        yield return gameplayCanvas.FadeIn();

        foreach (var unit in Activity.Units)
        {
            unit.Instance.OnMoveUnlocked += Instance_OnMoveUnlocked;
            var dancer = unit.GetComponent<UnitDance>();
            dancer.OnDanceMoveUsed += Dance_OnDanceMoveUsed;
            dancer.OnDanceMovesUpdated += Dance_OnDanceMovesUpdated;
            unit.SetBehaviour(dancer);
            LevelUI.UnitLevelViewMap[unit.Instance].SetColor(unit.Color);
        }

        SelectUnit(Activity.Units[0].GetComponent<UnitDance>());

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
                ShowTouchIndicator(Input.mousePosition);

                if (TryRaycastUnit(out UnitController unit) && Activity.Units.Contains(unit))
                {
                    var dancer = unit.GetComponent<UnitDance>();
                    if (selectedUnit != dancer)
                    {
                        SelectUnit(dancer);
                    }
                }
            }

            if (selectedUnit)
            {
                spotlight.transform.position = selectedUnit.transform.position + Vector3.up * 5f;
            }

            yield return null;
        }
    }

    private void Dance_OnDanceMovesUpdated()
    {
        if (selectedUnit)
        {
            StartCoroutine(danceMoveUIManager.Show(selectedUnit));
            //LevelUI.SetUnits(Activity.Units.Select(unit => unit.Instance));
        }
    }

    protected override void IncreaseXP(UnitController unit, float value)
    {
        if (gameplayCanvas.IsVisible)
        {
            base.IncreaseXP(unit, value);
        }
        else
        {
            unit.Instance.Skills[PrimarySkill].IncreaseSkillXP(value);
        }
    }

    private void SelectUnit(UnitDance unit)
    {
        UnselectUnit();

        selectedUnit = unit;
        selectedUnit.Highlight();
        spotlight.gameObject.SetActive(true);
        StartCoroutine(danceMoveUIManager.Show(selectedUnit));
    }

    private void Instance_OnMoveUnlocked(UnitInstance unit, DanceMove move)
    {
        Debug.Log($"{unit.name} unlocked {move.name}");
        StartCoroutine(LevelUpRoutine(unit, move));
    }

    private IEnumerator LevelUpRoutine(UnitInstance unit, DanceMove move)
    {
        yield return gameplayCanvas.FadeOut();
        yield return levelUpUI.Show(unit, unit.Skills[PrimarySkill], move);
    }

    private void Dance_OnDanceMoveUsed(UnitController unit, DanceMove danceMove)
    {
        IncreaseXP(unit, danceMove.Xp);
    }

    private void UnselectUnit()
    {
        if (selectedUnit)
        {
            selectedUnit.Unhighlight();
            spotlight.gameObject.SetActive(false);
        }
    }

    private UnitDance selectedUnit;

    protected override void OnActivityEnded()
    {
        base.OnActivityEnded();
        UnselectUnit();
        StopAllCoroutines();

        foreach (var unit in Activity.Units)
        {
            var dance = unit.GetComponent<UnitDance>();
            dance.OnDanceMoveUsed -= Dance_OnDanceMoveUsed;
        }
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
            touchIndicator.color = Color.Lerp(djReference.DominantTrack.Glyph.Color, Color.clear, t);
            yield return null;
        }

        touchIndicator.gameObject.SetActive(false);
    }
}
