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
    [SerializeField] private SkillLevelUI skillLevelUI;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        exitButton.onClick.AddListener(dancefloorReference.ExitDancefloor);
        skillLevelUI.OnLevelUp += SkillLevelUI_OnLevelUp;
    }

    private void SkillLevelUI_OnLevelUp()
    {
        StartCoroutine(LevelUpRoutine());
    }

    private IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(2f);
        skillLevelUI.Show(playerReference.Player.Instance);
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


        skillLevelUI.Show(playerReference.Player.Instance);

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
        var timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;


            if (timer > djReference.BeatDuration * 2f)
            {
                foreach (var dancer in dancefloorReference.Units)
                {
                    dancer.Instance.IncreaseSkillXP(Skill.DANCE, 1f);
                    IncreaseDanceXP(dancer.GetComponent<UnitDance>(), 1f);

                }

                timer = 0;
            }

            if (Input.GetMouseButtonDown(0))
            {
                playerReference.Player.GetComponent<UnitDance>().IncrementDancePower();
                IncreaseDanceXP(playerReference.Player.GetComponent<UnitDance>(), 1f);
            }

            yield return null;
        }
    }

    private void IncreaseDanceXP(UnitDance dancer, float value)
    {
        var worldPos = dancer.transform.position + Vector3.up;
        Vector3 viewportPos = background.DominantCamera.WorldToScreenPoint(worldPos);
        //Vector3 viewportPos = Camera.main.WorldToScreenPoint(worldPos);
        skillLevelUI.Increase(value, dancer.CurrentColor, viewportPos);
    }

    
}
