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
        //skillLevelUI.OnAllParticlesReached += SkillLevelUI_OnAllParticlesReached;
    }

    public override IEnumerator Show()
    {
        yield return base.Show();

        unit = dialogue.Unit;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(dialogue.Unit.transform.position);
        //skillLevelUI.SetUnit(unit.Instance, Skill.FRIENDSHIP);
        skillLevelUI.Increase(dialogue.Relationship, screenPos, SkillLevelUI_OnAllParticlesReached);
    }

    private void SkillLevelUI_OnAllParticlesReached(bool hasLeveledUp)
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
            //dialogue.StartChatDialogue(unit, new Dialogue("I really like your vibe, we should be friends!", type: DialogueType.FRIEND));
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
