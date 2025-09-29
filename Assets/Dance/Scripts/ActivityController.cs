using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class ActivityController : MonoBehaviour
{
    [SerializeField] private ActivityReference activity;
    [SerializeField] private Button exitButton;
    [SerializeField] private SkillLevelUIManager levelUI;
    [SerializeField] private Skill primarySkill;

    protected ActivityReference Activity => activity;
    protected SkillLevelUIManager LevelUI => levelUI;

    private Camera mainCamera;
    protected virtual Camera Camera => mainCamera;

    protected virtual void Awake()
    {
        mainCamera = Camera.main;
        exitButton.onClick.AddListener(activity.EndActivity);
    }

    private void OnEnable()
    {
        activity.OnActivityHasStarted += Activity_OnActivityHasStarted;
        activity.OnActivityHasEnded += OnActivityEnded;
    }

    private void OnDisable()
    {
        activity.OnActivityHasStarted -= Activity_OnActivityHasStarted;
        activity.OnActivityHasEnded -= OnActivityEnded;
    }

    private void Activity_OnActivityHasStarted()
    {
        StartCoroutine(OnActivityStartRoutine());
    }

    private IEnumerator OnActivityStartRoutine()
    {
        yield return new WaitUntil(() => levelUI.gameObject.activeInHierarchy);
        levelUI.SetUnits(activity.Units.Select(unit => unit.Instance));
        yield return new WaitUntil(() => activity.Units.All(unit => unit.IsAtDestination));
        yield return OnActivityStart();
    }

    protected abstract IEnumerator OnActivityStart();

    protected virtual void OnActivityEnded()
    {
        foreach (var unit in activity.Units)
        {
            unit.SetDefaultBehaviour();
        }
    }

    protected void IncreaseXP(UnitController unit, float value)
    {
        if (activity.Units.Contains(unit))
        {
            unit.Instance.Skills[primarySkill].IncreaseSkillXP(1f);

            var worldPos = unit.transform.position + Vector3.up;
            Vector3 viewportPos = Camera.WorldToScreenPoint(worldPos);

            LevelUI.UnitLevelViewMap[unit.Instance].SetColor(unit.Color);
            LevelUI.UnitLevelViewMap[unit.Instance].Increase(value, viewportPos);
        }
    }

}
