using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DancefloorGameplayUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DancefloorReference dancefloorReference;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private DancefloorEnergyController energyController;
    [SerializeField] private DancefloorBackground background;
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
        var dancers = dancefloorReference.Units.Select(unit => unit.GetComponent<UnitDance>()).ToList();
        dancers[0].StartDance();

        //energyController.touchColor = djReference.LeftTrack.Glyph.Color;

        //energyController.Activate(djReference.BeatDuration);
        StartCoroutine(DanceRoutine());
    }

    private void MusicVideoReference_OnMusicVideoEnd()
    {
        var dancers = dancefloorReference.Units.Select(unit => unit.GetComponent<UnitDance>()).ToList();
        dancers[0].EndDance();

        //energyController.Deactivate();
        StopAllCoroutines();
    }

    private IEnumerator DanceRoutine()
    {
        var dancers = dancefloorReference.Units.Select(unit => unit.GetComponent<UnitDance>()).ToList();

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var worldPos = dancers[0].transform.position + Vector3.up;

                Vector3 viewportPos = background.DominantCamera.WorldToScreenPoint(worldPos);

                //energyController.SendEnergy(viewportPos, djReference.LeftTrack.Glyph.Color);

                foreach(var dancer in dancers)
                {
                    dancer.IncrementDancePower();
                }
            }

            yield return null;
        }
    }

    
}
