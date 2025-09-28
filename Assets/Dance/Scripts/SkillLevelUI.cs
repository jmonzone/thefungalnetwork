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
        if (valueBarParticleController) valueBarParticleController.SetTargetColor(fillImage.color);
        if (fillImage) fillImage.fillAmount = Mathf.Lerp(0, 1, (instance.GetXP(skill) - instance.GetMinXP(skill)) / (instance.GetMaxXP(skill) - instance.GetMinXP(skill)));
        if (nextLevelText) nextLevelText.text = $"{instance.GetXPUntilNextLevel(skill)} xp until next level";
    }

    public void SetColor(Color color)
    {
        valueBarParticleController.SetStartColor(color);
    }

    public void Increase(float value, Vector3 screenPos)
    {
        valueBarParticleController.BurstFromWorld((int)value, screenPos);
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
        if (valueBarController)
        {
            valueBarController.SetTargetScale(1.1f);
            yield return new WaitForSeconds(1f);
        }

        if (fillImage)
        {
            yield return PulseFill();
        }

        if (nextLevelText) nextLevelText.text = $"{skill} Level Increased";

        OnLevelUp?.Invoke();
        levelUpRoutine = null;
        level = instance.GetLevel(skill);
    }

    private IEnumerator PulseFill()
    {
        if (!fillImage) yield break;

        float time = 0;

        var startScale = fillImage.transform.localScale.x;
        while (time < 1f)
        {
            float progress = time / 1f;
            float scale = Mathf.Lerp(startScale, startScale * 1.1f, Mathf.Sin(progress * Mathf.PI));
            fillImage.transform.localScale = Vector3.one * scale;

            time += Time.deltaTime;
            yield return null;
        }

        fillImage.transform.localScale = Vector3.one * startScale;
    }
}
