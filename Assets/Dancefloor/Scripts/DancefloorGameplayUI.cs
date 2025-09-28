using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DancefloorGameplayUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private DancefloorReference dancefloorReference;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private DancefloorBackground background;
    [SerializeField] private SkillLevelUIManager skillLevelUIManager;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        exitButton.onClick.AddListener(dancefloorReference.ExitDancefloor);
    }

    private void OnEnable()
    {
        dancefloorReference.OnDancefloorStart += MusicVideoReference_OnMusicVideoStart;
        dancefloorReference.OnDancefloorExit += MusicVideoReference_OnMusicVideoEnd;
    }

    private void OnDisable()
    {
        dancefloorReference.OnDancefloorStart -= MusicVideoReference_OnMusicVideoStart;
        dancefloorReference.OnDancefloorExit -= MusicVideoReference_OnMusicVideoEnd;
    }

    private void MusicVideoReference_OnMusicVideoStart()
    {
        foreach(var dancer in dancefloorReference.Dancers)
        {
            dancer.Unit.SetBehaviour(dancer);
        }

        skillLevelUIManager.SetUnits(dancefloorReference.Dancers.Select(unit => unit.Instance));

        foreach (var dancer in dancefloorReference.Dancers)
        {
            skillLevelUIManager.UnitLevelViewMap[dancer.Instance].SetColor(dancer.CurrentColor);
        }

        StartCoroutine(DanceRoutine());
    }

    private void MusicVideoReference_OnMusicVideoEnd()
    {
        foreach (var dancer in dancefloorReference.Dancers)
        {
            dancer.Unit.SetDefaultBehaviour();
        }

        StopAllCoroutines();
    }

    private IEnumerator DanceRoutine()
    {
        var timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;

            if (timer > djReference.BeatDuration)
            {
                foreach (var dancer in dancefloorReference.Dancers)
                {
                    IncreaseDanceXP(dancer, 1f);
                }

                timer = 0;
            }

            if (Input.GetMouseButtonDown(0))
            {
                dancefloorReference.Dancers[0].IncrementDancePower();
                IncreaseDanceXP(dancefloorReference.Dancers[0], 1f);
            }

            yield return null;
        }
    }

    private void IncreaseDanceXP(UnitDance dancer, float value)
    {
        dancer.Instance.IncreaseSkillXP(Skill.DANCE, 1f);

        var worldPos = dancer.transform.position + Vector3.up;
        Vector3 viewportPos = background.DominantCamera.WorldToScreenPoint(worldPos);
        skillLevelUIManager.UnitLevelViewMap[dancer.Instance].SetColor(dancer.CurrentColor);
        skillLevelUIManager.UnitLevelViewMap[dancer.Instance].Increase(value, viewportPos);
    }
}
