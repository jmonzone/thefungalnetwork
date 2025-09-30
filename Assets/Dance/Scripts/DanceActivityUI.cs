using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DanceActivityUI : ActivityController
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private DanceBackground background;
    [SerializeField] private DanceMoveUIManager danceMoveUIManager;
    [SerializeField] private Skill danceSkill;
    [SerializeField] private Image touchIndicator;
    [SerializeField] private Light spotlight;

    [Header("Settings")]
    [SerializeField] private float touchDuration = 0.2f;
    [SerializeField] private float touchScale = 1.5f;

    protected override Camera Camera => background.DominantCamera;

    protected override IEnumerator OnActivityStart()
    {
        foreach (var unit in Activity.Units)
        {
            var dance = unit.GetComponent<UnitDance>();
            dance.OnDanceMoveUsed += Dance_OnDanceMoveUsed;
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
                ShowTouchIndicator(Input.mousePosition);

                if (TryRaycastUnit(out UnitController unit) && Activity.Units.Contains(unit))
                {
                    if (unit != selectedUnit)
                    {
                        UnselectUnit();

                        selectedUnit = unit.GetComponent<UnitDance>();
                        selectedUnit.Highlight();

                        danceMoveUIManager.Show(selectedUnit);

                        spotlight.gameObject.SetActive(true);

                        IncreaseXP(selectedUnit.Unit, 1f);
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
