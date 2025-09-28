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
        foreach(var dancer in dancefloorReference.Units)
        {
            dancer.SetBehaviour(dancer.GetComponent<UnitDance>());
        }

        skillLevelUIManager.SetUnits(dancefloorReference.Units.Select(unit => unit.Instance));

        StartCoroutine(DanceRoutine());
    }

    private void MusicVideoReference_OnMusicVideoEnd()
    {
        foreach (var dancer in dancefloorReference.Units)
        {
            dancer.SetDefaultBehaviour();
        }

        StopAllCoroutines();
    }

    private IEnumerator DanceRoutine()
    {
        yield return new WaitForFixedUpdate();

        var timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;

            if (timer > djReference.BeatDuration * 2f)
            {
                foreach (var dancer in dancefloorReference.Units)
                {
                    IncreaseDanceXP(dancer, 1f);
                }

                timer = 0;
            }

            if (Input.GetMouseButtonDown(0))
            {
                playerReference.Player.GetComponent<UnitDance>().IncrementDancePower();
                IncreaseDanceXP(playerReference.Player, 1f);
            }

            yield return null;
        }
    }

    private void IncreaseDanceXP(UnitController unit, float value)
    {
        unit.Instance.IncreaseSkillXP(Skill.DANCE, 1f);

        var dancer = unit.GetComponent<UnitDance>();
        var worldPos = dancer.transform.position + Vector3.up;
        Vector3 viewportPos = background.DominantCamera.WorldToScreenPoint(worldPos);
        skillLevelUIManager.UnitLevelViewMap[unit.Instance].Increase(value, dancer.CurrentColor, viewportPos);
    }
}
