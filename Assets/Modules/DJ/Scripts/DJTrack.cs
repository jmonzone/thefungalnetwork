using UnityEngine;
using UnityEngine.Events;

namespace TheFungalNetwork.DJ
{
    public class DJTrack : MonoBehaviour
    {
        [SerializeField] public int trackIndex = 0;
        [SerializeField] private string groupId = "A";

        [SerializeField] private float volume = 1;
        [SerializeField] private float pitch = 1;
        [SerializeField] private float tempo = 1;

        [SerializeField] private DJTrack matchTrack;

        private DJTrackData data;
        private AudioSource audioSource;

        public AudioSource AudioSource => audioSource;
        public float Volume => volume;

        public DJTrackUI UI;

        public DJTrackData Data => data;
        public float Bpm => data.Bpm * tempo;

        public event UnityAction OnTrackChanged;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            SetVolume(volume);
            SetTempo(tempo);
            SetPitch(pitch);

            UI.OnVolumeSliderChanged += SetVolume;
            UI.OnTempoSliderChanged += SetTempo;
            UI.OnPitchSliderChanged += SetPitch;
            UI.OnToggle += value =>
            {
                if (value) AudioSource.Play();
                else AudioSource.Stop();
            };

            UI.OnSyncTempoButtonClicked += () =>
            {
                UI.SetBpm(matchTrack.Bpm);
            };
        }

        public void SetData(DJTrackData data, bool playImmediately)
        {
            this.data = data;
            audioSource.clip = data.AudioClip;
            if (playImmediately) audioSource.Play();
            else SetVolume(0);

            OnTrackChanged?.Invoke();
        }

        private void SetVolume(float linearVolume)
        {
            volume = linearVolume;
            //Debug.Log($"setting volume {groupId} {linearVolume}");
            audioSource.outputAudioMixerGroup.audioMixer.SetVolume(groupId, linearVolume);
        }

        private void SetTempo(float tempo)
        {
            this.tempo = tempo;
            UpdatePitch();
        }

        private void SetPitch(float pitch)
        {
            this.pitch = pitch;
            UpdatePitch();
        }

        private void UpdatePitch()
        {
            audioSource.pitch = tempo;
            //audioSource.outputAudioMixerGroup.audioMixer.SetFloat($"Pitch{groupId}", pitch / tempo);
        }

        public void SyncPitch()
        {
        }
    }
}
