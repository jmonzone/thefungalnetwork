using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class ActivityUI : MonoBehaviour
{
    [SerializeField] private ActivityReference activity;
    [SerializeField] private FadeCanvasGroup gameplayUI;
    [SerializeField] private SkillLevelUIManager levelUI;
    [SerializeField] private LevelUpUI levelUpUI;
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private Button exitButton;

    protected ActivityReference Activity => activity;
    protected SkillLevelUIManager LevelUI => levelUI;
    public bool IsGameplayUI => gameplayUI.IsVisible;

    protected virtual Camera Camera => mainCamera;

    private Camera mainCamera;

    protected virtual void Awake()
    {
        mainCamera = Camera.main;
        exitButton.onClick.AddListener(() =>
        {
            activity.ExitActivity(playerReference.ActivityUnit);
        });

        levelUpUI.gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        activity.OnUnitEnter += OnUnitEnter;
        activity.OnUnitExit += OnUnitExit;
        activity.OnPlayerEnter += OnPlayerEnter;
        activity.OnPlayerExit += OnPlayerExit;
    }

    protected virtual void OnDisable()
    {
        activity.OnUnitEnter -= OnUnitEnter;
        activity.OnUnitExit -= OnUnitExit;
        activity.OnPlayerEnter -= OnPlayerEnter;
        activity.OnPlayerExit -= OnPlayerExit;
    }

    protected virtual void OnUnitEnter(ActivityUnit unit)
    {
        levelUI.Show(activity.Units);
    }

    protected virtual void OnUnitExit(ActivityUnit unit)
    {
    }

    protected virtual void OnPlayerEnter(ActivityUnit player)
    {
        activity.OnXPIncreased += OnXPIncreased;
        StartCoroutine(gameplayUI.FadeIn());
    }

    protected virtual void OnPlayerExit(ActivityUnit player)
    {
        activity.OnXPIncreased -= OnXPIncreased;
    }

    private void OnXPIncreased(ActivityUnit unit, float value)
    {
        if (LevelUI.gameObject.activeInHierarchy)
        {
            Debug.Log("ActivityUI.OnXPIncreased");
            var unitWorldPos = unit.transform.position + Vector3.up * 0.5f;
            var unitScreenPos = Camera.WorldToScreenPoint(unitWorldPos);
            LevelUI.UnitLevelViewMap[unit].SetColor(unit.Color);
            LevelUI.UnitLevelViewMap[unit].Increase(value, unitScreenPos, hasLeveledUp =>
            {
                if (hasLeveledUp)
                {
                    StartCoroutine(LevelUpRoutine(unit));
                }
            });
        }
    }

    protected virtual IEnumerator LevelUpRoutine(ActivityUnit unit)
    {
        yield return gameplayUI.FadeOut();
        yield return levelUpUI.Show(unit, () =>
        {
            StartCoroutine(LevelUI_OnExitRoutine());
        });
    }

    protected virtual IEnumerator LevelUI_OnExitRoutine()
    {
        yield return levelUpUI.Hide();
        yield return gameplayUI.FadeIn();
    }

    protected void SetExitButtonInteractable(bool value)
    {
        exitButton.interactable = value;
    }
}
