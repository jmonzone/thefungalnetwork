using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DJTableReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference tracklistView;
    [SerializeField] private BackgroundMusicDelegate backgroundMusic;
    [SerializeField] private List<DJTrack> allTracks;

    [Header("Settings")]
    [SerializeField] private float crossFadeBeats;

    [Header("Runtime")]
    [SerializeField] private DJTableController controller;
    [SerializeField] private DJTrack leftTrack;
    [SerializeField] private DJTrack rightTrack;
    [SerializeField] private DJTrack dominantTrack;
    [SerializeField] private float leftValue = 0;
    [SerializeField] private float rightValue = 0;

    [SerializeField] private float bpm = 90;

    public List<DJTrack> Tracks => allTracks;
    public DJTableController Controller => controller;

    public DJTrack LeftTrack => leftTrack;
    public DJTrack RightTrack => rightTrack;
    public DJTrack DominantTrack => dominantTrack;

    public float LeftValue => leftValue;
    public float RightValue => rightValue;

    public float CrossFadeDuration => BeatDuration * crossFadeBeats;
    public float BPM => bpm;
    public float BeatDuration => 60f / bpm; // seconds per beat

    public event UnityAction OnBPMChanged;
    public event UnityAction<int> OnBeat;
    public event UnityAction OnTrackValueChanged;
    public event UnityAction<DJTrack> OnLeftTrackChanged;
    public event UnityAction<DJTrack> OnRightTrackChanged;
    public event UnityAction<DJTrack> OnDominantTrackChanged;

    public void Initialize()
    {
        dominantTrack = null;
    }

    public void RegisterController(DJTableController controller)
    {
        this.controller = controller;
    }

    public void StartTrack(DJTrack track)
    {
        backgroundMusic.HideMusic();
        SetTrackValue(0);
        SetBPM(track.Bpm);
        SetLeftTrack(track);
        dominantTrack = track;
    }

    public void SetLeftTrack(DJTrack track)
    {
        leftTrack = track;
        OnLeftTrackChanged?.Invoke(track);
    }

    public void SetRightTrack(DJTrack track)
    {
        rightTrack = track;
        OnRightTrackChanged?.Invoke(track);
    }

    public void SetTrackValue(float value)
    {
        leftValue = Mathf.Clamp(1 - value, 0, 1);
        rightValue = Mathf.Clamp(value, 0, 1);
        OnTrackValueChanged?.Invoke();

        var track = leftValue > 0.5 ? LeftTrack : RightTrack;
        if (track != dominantTrack)
        {
            dominantTrack = track;
            OnDominantTrackChanged?.Invoke(dominantTrack);
        }
    }

    public void SetBPM(float bpm)
    {
        this.bpm = bpm;
        OnBPMChanged?.Invoke();
    }

    public void InvokeBeat(int beat)
    {
        OnBeat?.Invoke(beat);
    }

    private int trackToSwap = 0;

    public void RequestSwapTrack(int value)
    {
        trackToSwap = value;
        navigation.Navigate(tracklistView);
    }

    public void SwapTrack(DJTrack track, int index = -1)
    {
        if (index < 0) index = trackToSwap;

        if (index == 0) SetLeftTrack(track);
        else SetRightTrack(track);

        navigation.GoBack();
    }
}
