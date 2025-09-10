public class DialogueFriendshipUI : DialoguePageUI
{
    private ValueBarController valueBarController;
    private ValueBarParticleController valueBarParticleController;

    protected override void Awake()
    {
        base.Awake();
        valueBarController = GetComponent<ValueBarController>();
        valueBarParticleController = GetComponent<ValueBarParticleController>();
        valueBarParticleController.OnParticleReached += ValueBarParticleController_OnParticlesReached;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void ValueBarParticleController_OnParticlesReached()
    {
        valueBarController.Increment();
    }

    public override void Show()
    {
        base.Show();

        valueBarController.SetTargetValue(dialogue.Unit.Instance.Relationship);
        dialogue.Unit.Instance.IncreaseRelationship(dialogue.Experience);
        valueBarParticleController.BurstFromWorld((int)dialogue.Experience, dialogue.Unit.transform.position);
    }
}
