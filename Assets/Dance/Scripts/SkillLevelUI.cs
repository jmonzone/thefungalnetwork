using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillLevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private ValueBarParticleManager valueBarParticleManager;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image unitImage;
    [SerializeField] private bool useAudio = true;

    private AudioSource audioSource;


    private float displayedXP;
    private int currentLevel;
    private bool hasLeveledUp;
    private Coroutine levelUpRoutine;

    public event UnityAction OnLevelUp;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        valueBarParticleManager.SetTargetColor(fillImage.color);
        valueBarParticleManager.OnParticleReached += ValueBarParticleController_OnParticleReached;
    }

    public void SetUnit(UnitController unit, UnitSkill skill)
    {
        unitImage.sprite = unit.Instance.Data.Sprite;
        SetDisplayedXP(skill.XP);
        SetColor(unit.Color);
    }

    public void Increase(float value, Vector3 screenPos, UnityAction<bool> onComplete)
    {
        var currentLevel = UnitSkill.GetLevelFromXP(displayedXP);

        valueBarParticleManager.BurstFromWorld((int)value, screenPos, () =>
        {
            var newLevel = UnitSkill.GetLevelFromXP(displayedXP);
            if (currentLevel != newLevel)
            {
                if (levelUpRoutine != null) StopCoroutine(levelUpRoutine);
                StartCoroutine(LevelUpRoutine());
            }
            onComplete?.Invoke(currentLevel != newLevel);
        });

        if (useAudio) audioSource.Play();
    }

    private void ValueBarParticleController_OnParticleReached()
    {
        SetDisplayedXP(displayedXP + 1);
    }

    private void SetDisplayedXP(float xp)
    {
        displayedXP = xp;
        currentLevel = UnitSkill.GetLevelFromXP(displayedXP); ;
        var minXP = UnitSkill.GetXPFromLevel(currentLevel);
        var maxXP = UnitSkill.GetXPFromLevel(currentLevel + 1);

        if (fillImage) fillImage.fillAmount = Mathf.Lerp(0, 1, (displayedXP - minXP) / (maxXP - minXP));
        levelText.text = $"Level {currentLevel}";
    }

    private IEnumerator LevelUpRoutine()
    {
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

        levelUpRoutine = null;
        hasLeveledUp = false;
        OnLevelUp?.Invoke();
    }


    public void SetColor(Color color)
    {
        valueBarParticleManager.SetStartColor(color);
    }
}
