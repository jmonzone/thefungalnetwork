using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class ActivityController : MonoBehaviour
{
    [SerializeField] private ActivityReference activity;
    [SerializeField] private Button exitButton;
    [SerializeField] private SkillLevelUIManager levelUI;

    protected ActivityReference Activity => activity;
    protected SkillLevelUIManager LevelUI => levelUI;

    private void Awake()
    {
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
        yield return new WaitUntil(() => activity.Units.All(unit => unit.IsAtDestination));
        levelUI.SetUnits(activity.Units.Select(unit => unit.Instance));
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

}
