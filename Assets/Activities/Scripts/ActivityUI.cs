using UnityEngine;
using UnityEngine.UI;

public abstract class ActivityUI : MonoBehaviour
{
    [SerializeField] private ActivityReference activity;
    [SerializeField] private SkillLevelUIManager levelUI;
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private Button exitButton;

    protected ActivityReference Activity => activity;
    protected SkillLevelUIManager LevelUI => levelUI;

    protected virtual Camera Camera => mainCamera;

    private Camera mainCamera;

    protected virtual void Awake()
    {
        mainCamera = Camera.main;
        exitButton.onClick.AddListener(() => activity.ExitActivity(playerReference.Player));
    }

    private void OnEnable()
    {
        activity.OnUnitEnter += Activity_OnUnitEnter;
        activity.OnPlayerEnter += OnPlayerEnter;
        activity.OnPlayerExit += OnPlayerExit;
    }

    private void Activity_OnUnitEnter(UnitController arg0)
    {
        //unit.Instance.Skills[PrimarySkill].OnMilestoneReached += Instance_OnMoveUnlocked;
    }

    private void OnDisable()
    {
        activity.OnPlayerEnter -= OnPlayerEnter;
        activity.OnPlayerExit -= OnPlayerExit;
        activity.OnUnitXpIncreased -= OnUnitXpIncreased;
    }

    protected virtual void OnPlayerEnter(PlayerController player)
    {
        levelUI.Show(activity.Units);
        activity.OnUnitXpIncreased += OnUnitXpIncreased;
    }
    protected virtual void OnPlayerExit(PlayerController player)
    {
        activity.OnUnitXpIncreased -= OnUnitXpIncreased;
    }

    private void OnUnitXpIncreased(UnitController unit, float value)
    {
        if (LevelUI.gameObject.activeInHierarchy)
        {
            var unitWorldPos = unit.transform.position + Vector3.up * 0.5f;
            var unitScreenPos = Camera.WorldToScreenPoint(unitWorldPos);
            LevelUI.UnitLevelViewMap[unit].SetColor(unit.Color);
            LevelUI.UnitLevelViewMap[unit].Increase(value, unitScreenPos);
        }
    }

    protected void SetExitButtonInteractable(bool value)
    {
        exitButton.interactable = value;
    }
}
