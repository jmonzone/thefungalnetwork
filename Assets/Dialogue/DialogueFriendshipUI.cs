using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueFriendshipUI : DialoguePageUI
{
    [SerializeField] private TextMeshProUGUI text;

    private ValueBarController valueBarController;
    private ValueBarParticleController valueBarParticleController;

    private int level;
    public bool HasLeveledUp => dialogue.Unit.Instance.RelationshipLevel > level;

    protected override void Awake()
    {
        base.Awake();
        valueBarController = GetComponent<ValueBarController>();
        valueBarParticleController = GetComponent<ValueBarParticleController>();
        valueBarParticleController.OnParticleReached += ValueBarParticleController_OnParticlesReached;
        valueBarParticleController.OnAllParticleReached += ValueBarParticleController_OnAllParticleReached;
    }

    public override void Show()
    {
        base.Show();

        var instance = dialogue.Unit.Instance;
        valueBarController.Initialize(instance.RelationshipPoints, instance.MinimumRelationshipPoints, instance.MaximumRelationshipPoints);
        level = instance.RelationshipLevel;
        instance.IncreaseRelationship(dialogue.Relationship);
        valueBarParticleController.BurstFromWorld((int)dialogue.Relationship, dialogue.Unit.transform.position);
    }

    private void Update()
    {
        text.color = valueBarController.AnimatedColor;
    }

    private void ValueBarParticleController_OnParticlesReached()
    {
        valueBarController.Increment();
    }

    private void ValueBarParticleController_OnAllParticleReached()
    {
        if (HasLeveledUp)
        {
            StartCoroutine(LevelUpRoutine());
        }
        else
        {
            StartCoroutine(CloseRoutine());
        }
    }

    private IEnumerator LevelUpRoutine()
    {
        valueBarController.SetTargetScale(1.1f);
        yield return new WaitForSeconds(1f);
        text.text = "Friendship Level Increased";
        yield return new WaitForSeconds(2f);
        dialogue.StartDialogue(dialogue.Unit, new Dialogue("I really like your vibe, we should be friends!", DialogueType.FRIEND));
    }

    private IEnumerator CloseRoutine()
    {
        yield return new WaitForSeconds(2f);
        InvokeClose();
    }
}
