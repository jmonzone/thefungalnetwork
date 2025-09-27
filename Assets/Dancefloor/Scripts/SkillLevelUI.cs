using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SkillLevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;
    [SerializeField] private Skill skill;
    [SerializeField] private bool useAudio = true;

    private UnitInstance instance;
    private int level;
    public bool HasLeveledUp => instance.GetLevel(skill) > level;

    private AudioSource audioSource;
    private ValueBarController valueBarController;
    private ValueBarParticleController valueBarParticleController;

    public event UnityAction OnLevelUp;
    public event UnityAction OnAllParticlesReached;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        nextLevelText.color = valueBarController.AnimatedColor;
    }

    public void Show(UnitInstance instance)
    {
        if (!valueBarController)
        {
            valueBarController = GetComponent<ValueBarController>();
            valueBarParticleController = GetComponent<ValueBarParticleController>();
            valueBarParticleController.OnParticleReached += ValueBarParticleController_OnParticlesReached;
            valueBarParticleController.OnAllParticleReached += ValueBarParticleController_OnAllParticleReached;
        }
       
        this.instance = instance;

        valueBarController.Initialize(instance.GetXP(skill), instance.GetMinXP(skill), instance.GetMaxXP(skill));
        levelText.text = $"Level {instance.GetLevel(skill)}";
        level = instance.GetLevel(skill);
        nextLevelText.text = $"{instance.GetXPUntilNextLevel(skill)} xp until next level";
    }

    public void Increase(float value, Color color, Vector3 screenPos)
    {
        valueBarParticleController.BurstFromWorld((int)value, color, screenPos);
        if (useAudio) audioSource.Play();
    }

    private void ValueBarParticleController_OnParticlesReached()
    {
        if (levelUpRoutine == null)
        {
            valueBarController.Increment();
            nextLevelText.text = $"{instance.GetXPUntilNextLevel(skill)} xp until next level";
        }
    }

    private Coroutine levelUpRoutine;
    private void ValueBarParticleController_OnAllParticleReached()
    {
        if (HasLeveledUp)
        {
            if (levelUpRoutine == null)
            {
                levelText.text = $"Level {instance.GetLevel(skill)}";
                levelUpRoutine = StartCoroutine(LevelUpRoutine());
            }
        }

        OnAllParticlesReached?.Invoke();
    }

    private IEnumerator LevelUpRoutine()
    {
        valueBarController.SetTargetScale(1.1f);
        yield return new WaitForSeconds(1f);
        nextLevelText.text = $"{skill} Level Increased";

        OnLevelUp?.Invoke();
        levelUpRoutine = null;
        level = instance.GetLevel(skill);

    }
}
