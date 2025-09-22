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
    [SerializeField] private string artist;
    [SerializeField] [TextArea] private string description;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private PartyMode partyMode;
    [SerializeField] private float bpm;
    [SerializeField] private GlyphData glyph;

    public string TrackName => trackName;
    public string Artist => artist;
    public string Description => description;
    public Sprite Sprite => glyph.Sprite;
    public PartyMode PartyMode => partyMode;
    public AudioClip AudioClip => audioClip;
    public float Bpm => bpm;

    public GlyphData Glyph => glyph;
}
