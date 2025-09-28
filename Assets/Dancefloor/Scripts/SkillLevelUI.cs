using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillLevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;
    [SerializeField] private ValueBarController valueBarController;
    [SerializeField] private ValueBarParticleController valueBarParticleController;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image unitImage;
    [SerializeField] private Skill skill;
    [SerializeField] private bool useAudio = true;

    private UnitInstance instance;
    private int level;
    public bool HasLeveledUp => instance.GetLevel(skill) > level;

    private AudioSource audioSource;

    public event UnityAction OnLevelUp;
    public event UnityAction OnAllParticlesReached;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        valueBarParticleController.OnParticleReached += ValueBarParticleController_OnParticlesReached;
        valueBarParticleController.OnAllParticleReached += ValueBarParticleController_OnAllParticleReached;
    }

    public void SetUnit(UnitInstance instance)
    {
        this.instance = instance;

        unitImage.sprite = instance.Data.Sprite;

        levelText.text = $"Level {instance.GetLevel(skill)}";
        level = instance.GetLevel(skill);

        if (valueBarController) valueBarController.Initialize(instance.GetXP(skill), instance.GetMinXP(skill), instance.GetMaxXP(skill));
        if (fillImage) fillImage.fillAmount = Mathf.Lerp(0, 1, (instance.GetXP(skill) - instance.GetMinXP(skill)) / (instance.GetMaxXP(skill) - instance.GetMinXP(skill)));
        if (nextLevelText) nextLevelText.text = $"{instance.GetXPUntilNextLevel(skill)} xp until next level";
    }

    public void Increase(float value, Color color, Vector3 screenPos)
    {
        valueBarParticleController.BurstFromWorld((int)value, color, fillImage.color, screenPos);
        if (useAudio) audioSource.Play();
    }

    private void ValueBarParticleController_OnParticlesReached()
    {
        if (levelUpRoutine == null)
        {
            if (valueBarController) valueBarController.Increment();
            if (fillImage) fillImage.fillAmount = Mathf.Lerp(0, 1, (instance.GetXP(skill) - instance.GetMinXP(skill)) / (instance.GetMaxXP(skill) - instance.GetMinXP(skill)));
            if (nextLevelText) nextLevelText.text = $"{instance.GetXPUntilNextLevel(skill)} xp until next level";
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
        if (valueBarController) valueBarController.SetTargetScale(1.1f);
        yield return new WaitForSeconds(1f);
        if (nextLevelText) nextLevelText.text = $"{skill} Level Increased";

        OnLevelUp?.Invoke();
        levelUpRoutine = null;
        level = instance.GetLevel(skill);

    }
}
