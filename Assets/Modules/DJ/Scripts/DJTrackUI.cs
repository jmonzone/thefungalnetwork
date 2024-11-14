using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TheFungalNetwork.DJ
{
    public class DJTrackUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI trackText;
        [SerializeField] private TextMeshProUGUI bpmText;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Slider pitchSlider;

        private DJTrackData track;
        private AudioSource audioSource;

        public event UnityAction OnSwapButtonClicked;

        private void Awake()
        {
            var swapButton = GetComponentInChildren<Button>();
            swapButton.onClick.AddListener(() => OnSwapButtonClicked?.Invoke());
        }

        public void Initialize(AudioSource audioSource, DJTrackData track, bool playImmediately)
        {
            this.audioSource = audioSource;
            audioSource.volume = playImmediately ? 1 : 0;
            audioSource.pitch = 1;

            volumeSlider.minValue = 0;
            volumeSlider.maxValue = 1;
            volumeSlider.value = audioSource.volume;
            volumeSlider.onValueChanged.AddListener(value =>
            {
                audioSource.volume = value;
            });

            pitchSlider.minValue = 0;
            pitchSlider.maxValue = 2;
            pitchSlider.value = audioSource.pitch;
            pitchSlider.onValueChanged.AddListener(value =>
            {
                audioSource.pitch = value;
                UpdateBPMText();
            });

            SetTrack(track);
        }

        public void SetTrack(DJTrackData track)
        {
            this.track = track;

            audioSource.clip = track.AudioClip;
            audioSource.Play();

            trackText.text = track.TrackName;
            UpdateBPMText();
        }

        private void UpdateBPMText()
        {
            if (!track) return;

            var bpm = Mathf.RoundToInt(track.Bpm * audioSource.pitch);
            bpmText.text = $"{bpm} bpm";
        }
    }
}