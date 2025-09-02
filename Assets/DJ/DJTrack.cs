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
    [SerializeField] private Sprite sprite;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private PartyMode partyMode;
    [SerializeField] private float bpm;

    [SerializeField] private Color noteColor;

    public string TrackName => trackName;
    public string TrackType => trackType;
    public Sprite Sprite => sprite;
    public PartyMode PartyMode => partyMode;
    public AudioClip AudioClip => audioClip;
    public float Bpm => bpm;

    public Color NoteColor => noteColor;
}
