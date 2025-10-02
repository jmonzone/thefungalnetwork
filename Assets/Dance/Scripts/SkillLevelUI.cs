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
    [SerializeField] private UnitSkill skill;
    [SerializeField] private bool useAudio = true;

    //public bool HasLeveledUp => skill.Level > currentLevel;

    private AudioSource audioSource;

    public event UnityAction OnLevelUp;
    public event UnityAction OnAllParticlesReached;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        valueBarParticleController.SetTargetColor(fillImage.color);
        valueBarParticleController.OnParticleReached += ValueBarParticleController_OnParticleReached;
        valueBarParticleController.OnAllParticleReached += ValueBarParticleController_OnAllParticleReached;
    }

    public void SetUnit(UnitInstance instance, UnitSkill skill)
    {
        this.skill = skill;

        unitImage.sprite = instance.Data.Sprite;

        //currentLevel = skill.Level;

        if (valueBarController) valueBarController.Initialize(skill.XP, skill.MinXP, skill.MaxXP);
        if (fillImage) fillImage.fillAmount = Mathf.Lerp(0, 1, (skill.XP - skill.MinXP) / (skill.MaxXP - skill.MinXP));
        UpdateView();
    }

    public void SetColor(Color color)
    {
        valueBarParticleController.SetStartColor(color);
    }

    public void Increase(float value, Vector3 screenPos, UnityAction onComplete)
    {
        valueBarParticleController.BurstFromWorld((int)value, screenPos, onComplete);
        if (useAudio) audioSource.Play();
    }

    private void ValueBarParticleController_OnParticleReached()
    {
        if (levelUpRoutine == null)
        {
            if (fillImage) fillImage.fillAmount = Mathf.Lerp(0, 1, (skill.XP - skill.MinXP) / (skill.MaxXP - skill.MinXP)); ;
            if (valueBarController) valueBarController.Increment();
            UpdateView();
        }
    }

    private void UpdateView()
    {
        levelText.text = $"Level {skill.Level}";
        if (nextLevelText) nextLevelText.text = $"{skill.XPUntilNextLevel} xp until next level";
    }

    private Coroutine levelUpRoutine;
    private void ValueBarParticleController_OnAllParticleReached()
    {
        //if (HasLeveledUp)
        //{
            if (levelUpRoutine == null)
            {
                UpdateView();
                //levelUpRoutine = StartCoroutine(LevelUpRoutine());
            }
        //}

        OnAllParticlesReached?.Invoke();
    }

    //private IEnumerator LevelUpRoutine()
    //{
    //    if (valueBarController)
    //    {
    //        valueBarController.SetTargetScale(1.1f);
    //        yield return new WaitForSeconds(1f);
    //    }

    //    if (fillImage)
    //    {
    //        yield return PulseFill();
    //    }

    //    if (nextLevelText) nextLevelText.text = $"{skill} Level Increased";

    //    OnLevelUp?.Invoke();
    //    levelUpRoutine = null;
    //    currentLevel = skill.Level;
    //}

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
