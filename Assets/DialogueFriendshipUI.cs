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

    private void ValueBarParticleController_OnParticlesReached()
    {
        valueBarController.Increment();
    }

    public override void Show()
    {
        base.Show();
        valueBarParticleController.BurstFromWorld(10, dialogue.Unit.transform.position);
    }
}
