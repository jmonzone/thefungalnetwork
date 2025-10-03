using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillLevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;
    [SerializeField] private ValueBarParticleManager valueBarParticleManager;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image unitImage;
    [SerializeField] private UnitSkill skill;
    [SerializeField] private bool useAudio = true;

    private AudioSource audioSource;

    public event UnityAction OnLevelUp;
    public event UnityAction OnAllParticlesReached;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        valueBarParticleManager.SetTargetColor(fillImage.color);
        valueBarParticleManager.OnParticleReached += ValueBarParticleController_OnParticleReached;
        valueBarParticleManager.OnAllParticleReached += ValueBarParticleController_OnAllParticleReached;
    }

    public void SetUnit(UnitController unit, UnitSkill skill)
    {
        this.skill = skill;

        unitImage.sprite = unit.Instance.Data.Sprite;
        SetDisplayedXP(skill.XP);
        SetColor(unit.Color);
        UpdateView();
    }

    public void SetColor(Color color)
    {
        valueBarParticleManager.SetStartColor(color);
    }

    private float displayedXP;

    public void Increase(float value, Vector3 screenPos)
    {
        valueBarParticleManager.BurstFromWorld((int)value, screenPos);
        if (useAudio) audioSource.Play();
    }

    private void ValueBarParticleController_OnParticleReached()
    {
        SetDisplayedXP(displayedXP + 1);

        if (levelUpRoutine == null)
        {
            UpdateView();
        }
    }

    private void SetDisplayedXP(float xp)
    {
        displayedXP = xp;
        var level = UnitSkill.GetLevelFromXP(displayedXP);
        var minXP = UnitSkill.GetXPFromLevel(level);
        var maxXP = UnitSkill.GetXPFromLevel(level + 1);

        if (fillImage) fillImage.fillAmount = Mathf.Lerp(0, 1, (displayedXP - minXP) / (maxXP - minXP));
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
