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
        [SerializeField] private Toggle toggle;
        [SerializeField] private Button syncButton;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Slider pitchSlider;

        private Track track;

        public event UnityAction OnSwapButtonClicked;
        public event UnityAction OnSyncButtonClicked;
        public event UnityAction<bool> OnToggle;
        public event UnityAction<float> OnVolumeSliderChanged;

        private void Awake()
        {
            swapButton.onClick.AddListener(() => OnSwapButtonClicked?.Invoke());
            syncButton.onClick.AddListener(() => OnSyncButtonClicked?.Invoke());
            toggle.onValueChanged.AddListener(value => OnToggle?.Invoke(value));
        }

        public void Initialize(Track track, bool playImmediately)
        {
            this.track = track;
            track.audioSource.pitch = 1;

            toggle.isOn = playImmediately;

            volumeSlider.minValue = 0;
            volumeSlider.maxValue = 1;
            volumeSlider.value = playImmediately ? 1 : 0;
            volumeSlider.onValueChanged.AddListener(value =>
            {
                OnVolumeSliderChanged?.Invoke(value);
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

            trackText.text = track.Data.TrackName;
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