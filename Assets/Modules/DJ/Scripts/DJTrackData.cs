using UnityEngine;

namespace TheFungalNetwork.DJ
{
    public enum MusicalKey
    {
        C = 0,
        CSharp = 1,
        D = 2,
        DSharp = 3,
        E = 4,
        F = 5,
        FSharp = 6,
        G = 7,
        GSharp = 8,
        A = 9,
        ASharp = 10,
        B = 11
    }

    [CreateAssetMenu]
    public class DJTrackData : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private int bpm;
        [SerializeField] private MusicalKey key;

        public AudioClip AudioClip => audioClip;
        public string TrackName => name;
        public int Bpm => bpm;
        public MusicalKey Key => key;
    }
}