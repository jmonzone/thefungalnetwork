using UnityEngine;

public enum PartyMode
{
    Regular,
    Alternating,
    Strobe
}

[CreateAssetMenu]
public class DJTrack : ScriptableObject
{
    [SerializeField] private string trackName;
    [SerializeField] private string trackType;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private PartyMode partyMode;
    [SerializeField] private float bpm;

    public string TrackName => trackName;
    public string TrackType => trackType;
    public PartyMode PartyMode => partyMode;
    public AudioClip AudioClip => audioClip;
    public float Bpm => bpm;
}
