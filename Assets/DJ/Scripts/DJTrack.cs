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
    [SerializeField] private Sprite sprite;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private PartyMode partyMode;
    [SerializeField] private float bpm;

    [SerializeField] private Color noteColor;
    [SerializeField] private GlyphData glyph;

    public string TrackName => trackName;
    public string Artist => artist;
    public string Description => description;
    public Sprite Sprite => sprite;
    public PartyMode PartyMode => partyMode;
    public AudioClip AudioClip => audioClip;
    public float Bpm => bpm;

    public Color NoteColor => noteColor;
    public GlyphData Glyph => glyph;
}
