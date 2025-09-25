using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DancefloorGameplayUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DancefloorReference dancefloorReference;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private DancefloorBeatManager beatManager;
    [SerializeField] private DancefloorAuraController auraController;
    [SerializeField] private DancefloorEnergyController energyController;
    [SerializeField] private DancefloorBackground background;
    [SerializeField] private RectTransform energyRect;
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
        //beatManager.StartBeats();
        auraController.auraColor = djReference.LeftTrack.Glyph.Color;
        auraController.beatInterval = djReference.BeatDuration;
        energyController.touchColor = djReference.LeftTrack.Glyph.Color;

        auraController.Activate();
        energyController.Activate(djReference.BeatDuration);
        StartCoroutine(DanceRoutine());
    }

    private void MusicVideoReference_OnMusicVideoEnd()
    {
        //beatManager.StopBeats();
        auraController.Deactivate();
        energyController.Deactivate();
        StopAllCoroutines();
    }

    private IEnumerator DanceRoutine()
    {
        var dancers = dancefloorReference.Units.Select(unit => unit.GetComponent<UnitDance>()).ToList();

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var worldPos = dancers[0].transform.position + Vector3.up * 3f;

                Vector3 viewportPos = background.DominantCamera.WorldToScreenPoint(worldPos);

                energyController.SendEnergy(viewportPos + Vector3.right * 200f + Vector3.up * 100f);

                foreach(var dancer in dancers)
                {
                    dancer.StartDance();
                }
            }

            yield return null;
        }
    }

    
}
