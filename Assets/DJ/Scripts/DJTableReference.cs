using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DJTableReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference tracklistView;
    [SerializeField] private List<DJTrack> allTracks;

    [Header("Runtime")]
    [SerializeField] private DJTableController djTable;
    [SerializeField] private DJTrack leftTrack;
    [SerializeField] private DJTrack rightTrack;
    [SerializeField] private float leftValue;
    [SerializeField] private float rightValue;

    [SerializeField] private float bpm = 90;

    public List<DJTrack> Tracks => allTracks;
    public DJTableController DjTable => djTable;
    public DJTrack LeftTrack => leftTrack;
    public DJTrack RightTrack => rightTrack;
    public float LeftValue => leftValue;
    public float RightValue => rightValue;

    public float BPM => bpm;
    public float BeatDuration => 60f / bpm; // seconds per beat

    public event UnityAction OnBPMChanged;
    public event UnityAction<int> OnBeat;
    public event UnityAction OnLeftTrackChanged;
    public event UnityAction OnRightTrackChanged;

    public void Initialize()
    {
        bpm = 90;
        djTable = null;

        leftTrack = allTracks[0];
        rightTrack = allTracks[1];
        SetTrackValue(0);
    }

    public void SetDJTable(DJTableController djTable)
    {
        this.djTable = djTable;
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

    public void SetLeftTrack(DJTrack track)
    {
        leftTrack = track;
        OnLeftTrackChanged?.Invoke();
    }

    public void SetRightTrack(DJTrack track)
    {
        rightTrack = track;
        OnRightTrackChanged?.Invoke();
    }

    public void SetTrackValue(float value)
    {
        leftValue = 1 - value;
        rightValue = value;
    }

    private int trackToSwap = 0;

    public void RequestSwapTrack(int value)
    {
        trackToSwap = value;
        navigation.Navigate(tracklistView);
    }

    public void SwapTrack(DJTrack track)
    {
        if (trackToSwap == 0) SetLeftTrack(track);
        else SetRightTrack(track);

        navigation.GoBack();
    }
}
