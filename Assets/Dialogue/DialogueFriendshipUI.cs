using System.Collections;
using UnityEngine;

public class DialogueFriendshipUI : DialoguePageUI
{
    [SerializeField] private SkillLevelUI skillLevelUI;
    [SerializeField] private Color color;

    public bool HasLeveledUp { get; private set; }

    private UnitController unit;

    protected override void Awake()
    {
        base.Awake();
        skillLevelUI.OnLevelUp += SkillLevelUI_OnLevelUp;
        skillLevelUI.OnAllParticlesReached += SkillLevelUI_OnAllParticlesReached;
    }

    public override void Show()
    {
        base.Show();

        unit = dialogue.Unit;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(dialogue.Unit.transform.position);
        skillLevelUI.Show(unit.Instance);
        skillLevelUI.Increase(dialogue.Relationship, color, screenPos);
    }

    private void SkillLevelUI_OnAllParticlesReached()
    {
        StartCoroutine(CloseRoutine());
    }

    private void SkillLevelUI_OnLevelUp()
    {
        StartCoroutine(LevelUpRoutine());
    }

    private IEnumerator LevelUpRoutine()
    {
        HasLeveledUp = true;
        if (dialogue.Unit.Instance.FriendshipLevel == 2)
        {
            yield return new WaitForSeconds(2f);
            dialogue.StartDialogue(unit, new Dialogue("I really like your vibe, we should be friends!", type: DialogueType.FRIEND));
        }
        else
        {
            yield return CloseRoutine();
        }

        HasLeveledUp = false;
    }

    private IEnumerator CloseRoutine()
    {
        yield return new WaitForSeconds(2f);
        InvokeClose();
    }
}
