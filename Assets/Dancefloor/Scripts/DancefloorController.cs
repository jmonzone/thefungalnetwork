using UnityEngine;

public class DancefloorController : MonoBehaviour
{
    [SerializeField] private DancefloorReference musicVideoReference;
    [SerializeField] private PlayerReference playerReference;

    [SerializeField] private ZoneController zoneController;

    private void Awake()
    {
        zoneController.OnPlayerEnterZone += musicVideoReference.EnterDancefloor;
    }

    private void OnEnable()
    {
        musicVideoReference.OnDancefloorExit += MusicVideoReference_OnMusicVideoEnd;
    }

    private void OnDisable()
    {
        musicVideoReference.OnDancefloorExit -= MusicVideoReference_OnMusicVideoEnd;
    }

    private void MusicVideoReference_OnMusicVideoEnd()
    {
        playerReference.SetTargetPosition(zoneController.EntryPosition);
    }
}
