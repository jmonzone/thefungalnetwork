using UnityEngine;

namespace TheFungalNetwork.DJ
{
    [CreateAssetMenu]
    public class DJTrackData : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private int bpm;

        public AudioClip AudioClip => audioClip;
        public string TrackName => name;
        public int Bpm => bpm;
    }
}