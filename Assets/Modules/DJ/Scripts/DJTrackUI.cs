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
        [SerializeField] private Button syncTempoButton;
        [SerializeField] private Button syncPitchButton;

        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Slider tempoSlider;
        [SerializeField] private Slider pitchSlider;

        private DJTrack track;

        public event UnityAction OnSwapButtonClicked;
        public event UnityAction OnSyncTempoButtonClicked;

        public event UnityAction<bool> OnToggle;
        public event UnityAction<float> OnVolumeSliderChanged;
        public event UnityAction<float> OnTempoSliderChanged;
        public event UnityAction<float> OnPitchSliderChanged;

        private void Awake()
        {
            swapButton.onClick.AddListener(() => OnSwapButtonClicked?.Invoke());
            syncTempoButton.onClick.AddListener(() => OnSyncTempoButtonClicked?.Invoke());
            toggle.onValueChanged.AddListener(value => OnToggle?.Invoke(value));
            syncPitchButton.onClick.AddListener(() => track.SyncPitch());
        }

        public void Initialize(DJTrack track, bool playImmediately)
        {
            this.track = track;

            toggle.isOn = playImmediately;

            volumeSlider.minValue = 0;
            volumeSlider.maxValue = 1;
            volumeSlider.value = playImmediately ? 1 : 0;
            volumeSlider.onValueChanged.AddListener(value =>
            {
                OnVolumeSliderChanged?.Invoke(value);
            });

            tempoSlider.minValue = 0;
            tempoSlider.maxValue = 2;
            tempoSlider.value = 1;
            tempoSlider.onValueChanged.AddListener(value =>
            {
                OnTempoSliderChanged?.Invoke(value);
                UpdateBPMText();
            });

            pitchSlider.minValue = 0;
            pitchSlider.maxValue = 2;
            pitchSlider.value = 1;
            pitchSlider.onValueChanged.AddListener(value =>
            {
                OnPitchSliderChanged?.Invoke(value);
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
            tempoSlider.value = bpm / track.Data.Bpm;
        }


        private void UpdateBPMText()
        {
            var roundedBpm = Mathf.RoundToInt(track.Bpm);
            bpmText.text = $"{roundedBpm} bpm";
        }
    }
}