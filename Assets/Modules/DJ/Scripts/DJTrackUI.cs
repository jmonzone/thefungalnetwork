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
        [SerializeField] private Button swapButton;
        [SerializeField] private Button syncButton;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Slider pitchSlider;

        private Track track;

        public event UnityAction OnSwapButtonClicked;
        public event UnityAction OnSyncButtonClicked;

        private void Awake()
        {
            swapButton.onClick.AddListener(() => OnSwapButtonClicked?.Invoke());
            syncButton.onClick.AddListener(() => OnSyncButtonClicked?.Invoke());
        }

        public void Initialize(Track track, bool playImmediately)
        {
            this.track = track;
            track.audioSource.volume = playImmediately ? 1 : 0;
            track.audioSource.pitch = 1;

            volumeSlider.minValue = 0;
            volumeSlider.maxValue = 1;
            volumeSlider.value = track.audioSource.volume;
            volumeSlider.onValueChanged.AddListener(value =>
            {
                track.audioSource.volume = value;
            });

            pitchSlider.minValue = 0;
            pitchSlider.maxValue = 2;
            pitchSlider.value = track.audioSource.pitch;
            pitchSlider.onValueChanged.AddListener(value =>
            {
                track.audioSource.pitch = value;
                UpdateBPMText();
            });


            track.OnTrackChanged += () =>
            {
                trackText.text = track.Data.TrackName;
                UpdateBPMText();
            };

            UpdateBPMText();
        }

        public void SetBpm(float bpm)
        {
            pitchSlider.value = bpm / track.Data.Bpm;
        }


        private void UpdateBPMText()
        {
            var roundedBpm = Mathf.RoundToInt(track.Bpm);
            bpmText.text = $"{roundedBpm} bpm";
        }
    }
}