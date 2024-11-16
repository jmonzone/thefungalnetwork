using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace TheFungalNetwork.DJ
{
    public static class AudioMixerExtensions
    {
        public static void SetVolume(this AudioMixer audioMixer, string groupId, float linearVolume)
        {
            float decibalVolume = Mathf.Log10(Mathf.Clamp(linearVolume, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat($"Volume{groupId}", decibalVolume);
        }
    }

    public class DJTable : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;

        [Header("Audio References")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private List<DJTrackData> tracks;
        [SerializeField] private DJTrack track1;
        [SerializeField] private DJTrack track2;


        [Header("Transition References")]
        [SerializeField] private GameObject inputCanvas;
        [SerializeField] private GameObject djCanvas;
        [SerializeField] private Button exitButton;
        [SerializeField] private AudioSource backgroundMusic;
        [SerializeField] private Animator visualsAnimator;

        private OverheadInteractionIndicator overheadInteraction;
        private CinemachineVirtualCamera virtualCamera;

        private void Awake()
        {
            overheadInteraction = GetComponentInChildren<OverheadInteractionIndicator>();
            virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

            var proximityAction = GetComponentInChildren<ProximityAction>();
            proximityAction.OnUse += Use;

            exitButton.onClick.AddListener(Exit);

            ToggleView(false);
        }

        private void Start()
        {
            InitializeTrack(track1, track2, true);
            InitializeTrack(track2, track1, false);
        }

        private void InitializeTrack(DJTrack track, DJTrack syncTrack, bool playImmediately)
        {
            track.trackIndex %= tracks.Count;
            track.SetData(tracks[track.trackIndex], playImmediately);

            track.UI.Initialize(track, playImmediately);
            track.UI.OnSwapButtonClicked += () =>
            {
                track.trackIndex = (track.trackIndex + 1) % tracks.Count;
                track.SetData(tracks[track.trackIndex], true);
            };
        }

        private void Update()
        {
            bool isTrack1Playing = track1.AudioSource.isPlaying && track1.Volume > 0.1f;
            bool isTrack2Playing = track2.AudioSource.isPlaying && track2.Volume > 0.1f;

            if (isTrack1Playing || isTrack2Playing)
            {
                // Determine the loudest active track
                DJTrack targetTrack;

                if (isTrack1Playing && isTrack2Playing)
                {
                    targetTrack = track1.Volume >= track2.Volume ? track1 : track2;
                }
                else if (isTrack1Playing)
                {
                    targetTrack = track1;
                }
                else
                {
                    targetTrack = track2;
                }

                // Update visuals animator speed based on the active track's BPM and pitch
                visualsAnimator.speed = (tracks[targetTrack.trackIndex].Bpm * targetTrack.AudioSource.pitch) / 30;
            }
            else
            {
                visualsAnimator.speed = 0;
            }




            float distance = Vector3.Distance(inputManager.Controllable.Movement.transform.position, transform.position);
            float maxDistance = 10f; // Adjust this value to control the range for volume falloff
            float minDistance = 3f;  // Range within which volume will be 1

            if (distance <= minDistance)
            {
                audioMixer.SetVolume("Master", 1f);
            }
            else
            {
                float volume = Mathf.Clamp01(1 - Mathf.Log10(distance - minDistance + 1) / Mathf.Log10(maxDistance - minDistance + 1));
                audioMixer.SetVolume("Master", volume);
            }
        }

        private void Use()
        {
            ToggleView(true);
            backgroundMusic.Stop();
        }
        private void Exit()
        {
            ToggleView(false);
            backgroundMusic.Play();
        }

        private void ToggleView(bool value)
        {
            djCanvas.SetActive(value);
            inputCanvas.SetActive(!value);
            overheadInteraction.gameObject.SetActive(!value);
            virtualCamera.Priority = value ? 2 : 0;
        }
    }
}
