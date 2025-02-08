using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Video;

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
        [SerializeField] private Controller controller;

        [Header("Audio References")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private List<DJTrackData> tracks;
        [SerializeField] private DJTrack track1;
        [SerializeField] private DJTrack track2;
        [SerializeField] private float maxDistance = 10f; // Adjust this value to control the range for volume falloff
        [SerializeField] private float minDistance = 4f;  // Range within which volume will be 1

        [Header("Visual Refrences")]
        [SerializeField] private VideoPlayer visualsVideoPlayer;
        [SerializeField] private GameObject fullscreenVisuals;
        [SerializeField] private Button openVisualsButton;
        [SerializeField] private Button closeVisualsButton;

        private void Awake()
        {
            openVisualsButton.onClick.AddListener(() => fullscreenVisuals.SetActive(true));
            closeVisualsButton.onClick.AddListener(() => fullscreenVisuals.SetActive(false));
        }

        private void Start()
        {
            InitializeTrack(track1, true);
            InitializeTrack(track2, false);
        }

        //todo: centralize initialization track into DJTrack
        private void InitializeTrack(DJTrack track, bool playImmediately)
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
                visualsVideoPlayer.playbackSpeed = targetTrack.Bpm / 120;

                //float distance = Vector3.Distance(controller.Movement.transform.position, transform.position);


                //if (distance <= minDistance)
                //{
                //    audioMixer.SetVolume("Background", 0f);
                //    audioMixer.SetVolume("DJ Table", 1f);
                //}
                //else
                //{
                //    float volume = Mathf.Clamp01(1 - Mathf.Log10(distance - minDistance + 1) / Mathf.Log10(maxDistance - minDistance + 1));
                //    audioMixer.SetVolume("DJ Table", volume);
                //    audioMixer.SetVolume("Background", 1 - volume);

                //}
            }
            else
            {
                visualsVideoPlayer.playbackSpeed = 0;
            }
        }
    }
}
