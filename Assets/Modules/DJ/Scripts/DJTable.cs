using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TheFungalNetwork.DJ
{
    [Serializable]
    public class Track
    {
        public int index;
        private DJTrackData data;
        public DJTrackUI UI;
        public AudioSource audioSource;

        public DJTrackData Data => data;
        public float Bpm => data.Bpm * audioSource.pitch;

        public event UnityAction OnTrackChanged;

        public void SetData(DJTrackData data, bool playImmediately)
        {
            this.data = data;
            audioSource.clip = data.AudioClip;
            if (playImmediately) audioSource.Play();
            OnTrackChanged?.Invoke();
        }
    }

    public class DJTable : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;

        [Header("UI References")]
        [SerializeField] private List<DJTrackData> tracks;
        [SerializeField] private Track track1;
        [SerializeField] private Track track2;

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

            InitializeTrack(track1, track2, true);
            InitializeTrack(track2, track1, false);
        }

        private void InitializeTrack(Track track, Track syncTrack, bool playImmediately)
        {
            track.index %= tracks.Count;
            track.SetData(tracks[track.index], playImmediately);

            track.UI.Initialize(track, playImmediately);
            track.UI.OnSwapButtonClicked += () =>
            {
                track.index = (track.index + 1) % tracks.Count;
                track.SetData(tracks[track.index], true);
            };

            track.UI.OnSyncButtonClicked += () =>
            {
                Debug.Log($"Syncing {track.Data.Bpm} {syncTrack.Data.Bpm} {track.Bpm} {syncTrack.Bpm}");
                track.UI.SetBpm(syncTrack.Bpm);
            };
        }

        private void Update()
        {
            var loudestTrack = track1.audioSource.volume >= track2.audioSource.volume ? track1 : track2;
            visualsAnimator.speed = tracks[loudestTrack.index].Bpm * loudestTrack.audioSource.pitch / 30;

            //float distance = Vector3.Distance(inputManager.Controllable.Movement.transform.position, transform.position);
            //float maxDistance = 15f; // Adjust this value to control the range for volume falloff
            //float minDistance = 3f;  // Range within which volume will be 1

            //    if (distance <= minDistance)
            //    {
            //        audioSource.volume = 1f;
            //    }
            //    else
            //    {
            //        float volume = Mathf.Clamp01(1 - Mathf.Log10(distance - minDistance + 1) / Mathf.Log10(maxDistance - minDistance + 1));
            //        audioSource.volume = volume;
            //    }
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
