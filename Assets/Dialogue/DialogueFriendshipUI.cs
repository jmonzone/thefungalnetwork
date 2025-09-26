using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueFriendshipUI : DialoguePageUI
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;

    private AudioSource audioSource;
    private ValueBarController valueBarController;
    private ValueBarParticleController valueBarParticleController;

    private int level;
    public bool HasLeveledUp => dialogue.Unit.Instance.FriendshipLevel > level;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        valueBarController = GetComponent<ValueBarController>();
        valueBarParticleController = GetComponent<ValueBarParticleController>();
        valueBarParticleController.OnParticleReached += ValueBarParticleController_OnParticlesReached;
        valueBarParticleController.OnAllParticleReached += ValueBarParticleController_OnAllParticleReached;
    }

    public override void Show()
    {
        base.Show();

        var instance = dialogue.Unit.Instance;

        levelText.text = $"Level {instance.FriendshipLevel}";

        valueBarController.Initialize(instance.FriendshipPoints, instance.MinFP, instance.MaxFP);
        level = instance.FriendshipLevel;
        instance.IncreaseFriendship(dialogue.Relationship);
        valueBarParticleController.BurstFromWorld((int)dialogue.Relationship, dialogue.Unit.transform.position);
        audioSource.Play();

    }

    private void Update()
    {
        nextLevelText.color = valueBarController.AnimatedColor;
    }

    private void ValueBarParticleController_OnParticlesReached()
    {
        valueBarController.Increment();
    }

    private void ValueBarParticleController_OnAllParticleReached()
    {
        if (HasLeveledUp)
        {
            levelText.text = $"Level {dialogue.Unit.Instance.FriendshipLevel}";
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
        nextLevelText.text = "Friendship Level Increased";

        if (dialogue.Unit.Instance.FriendshipLevel == 2)
        {
            yield return new WaitForSeconds(2f);
            dialogue.StartDialogue(dialogue.Unit, new Dialogue("I really like your vibe, we should be friends!", type: DialogueType.FRIEND));
        }
        else
        {
            yield return CloseRoutine();
        }
    }

    private IEnumerator CloseRoutine()
    {
        yield return new WaitForSeconds(2f);
        InvokeClose();
    }
}
