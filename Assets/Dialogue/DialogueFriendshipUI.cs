using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueFriendshipUI : DialoguePageUI
{
    [SerializeField] private TextMeshProUGUI text;
    private ValueBarController valueBarController;
    private ValueBarParticleController valueBarParticleController;

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

        valueBarController.Initialize(dialogue.Unit.Instance.Relationship, 0, 8);
        dialogue.Unit.Instance.IncreaseRelationship(dialogue.Relationship);
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
        StartCoroutine(LevelUpRoutine());
    }

    private IEnumerator LevelUpRoutine()
    {
        valueBarController.SetTargetScale(1.25f);
        yield return new WaitForSeconds(1f);
        text.text = "Friendship Level Increased";
        yield return new WaitForSeconds(2f);
        dialogue.StartDialogue(dialogue.Unit, new Dialogue("I really like your vibe, we should be friends!", DialogueType.FRIEND));
    }
}
