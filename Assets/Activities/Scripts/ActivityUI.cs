using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class ActivityUI<T1, T2> : MonoBehaviour where T1 : ActivityBehaviour where T2: ActivityController<T1>
{
    [SerializeField] private ActivityReference activity;
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private T1 player;
    [SerializeField] private T2 controller;

    [SerializeField] private SkillLevelUIManager levelUI;
    [SerializeField] private FadeCanvasGroup gameplayUI;
    [SerializeField] private LevelUpUI levelUpUI;
    [SerializeField] private Button exitButton;

    private Camera mainCamera;

    protected virtual Camera Camera => mainCamera;

    protected ActivityReference Activity => activity;
    protected PlayerReference PlayerReference => playerReference;
    protected T1 Player => player;
    protected T2 Controller => controller;
    protected bool PlayerIsSelected => player == controller.CurrentUnit;

    protected SkillLevelUIManager LevelUI => levelUI;
    public bool IsGameplayUI => gameplayUI.IsVisible;

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
        this.player = player.GetComponent<T1>();
        activity.OnXPIncreased += OnXPIncreased;
        controller.OnUnitSelected += OnUnitSelected;

        StartCoroutine(gameplayUI.FadeIn());
    }

    protected virtual void OnPlayerExit(ActivityUnit player)
    {
        this.player = null;
        activity.OnXPIncreased -= OnXPIncreased;
        controller.OnUnitSelected -= OnUnitSelected;
    }

    protected virtual void OnUnitSelected(T1 unit)
    {
    }

    private void OnXPIncreased(OnXpIncreasedEventArgs args)
    {
        if (LevelUI.gameObject.activeInHierarchy)
        {
            //Debug.Log("ActivityUI.OnXPIncreased");
            var unitWorldPos = args.SourcePosition;
            var unitScreenPos = Camera.WorldToScreenPoint(unitWorldPos);
            LevelUI.UnitLevelViewMap[args.Unit].SetColor(args.Unit.Color);
            LevelUI.UnitLevelViewMap[args.Unit].Increase(args.XP, unitScreenPos, hasLeveledUp =>
            {
                if (hasLeveledUp)
                {
                    StartCoroutine(LevelUpRoutine(args.Unit));
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
        levelUI.Show(activity.Units);
    }

    protected void SetExitButtonInteractable(bool value)
    {
        exitButton.interactable = value;
    }
}
