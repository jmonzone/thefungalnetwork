using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DJTableReference : ScriptableObject
{
    [Header("Runtime")]
    [SerializeField] private DJTableController djTable;
    [SerializeField] private DJTrack leftTrack;
    [SerializeField] private DJTrack rightTrack;
    [SerializeField] private float leftValue;
    [SerializeField] private float rightValue;

    [SerializeField] private float bpm = 90;

    public DJTableController DjTable => djTable;
    public DJTrack LeftTrack => leftTrack;
    public DJTrack RightTrack => rightTrack;
    public float LeftValue => leftValue;
    public float RightValue => rightValue;

    public float BPM => bpm;
    public float BeatDuration => 60f / bpm; // seconds per beat

    public event UnityAction OnBPMChanged;
    public event UnityAction<int> OnBeat;

    public void Initialize()
    {
        bpm = 90;
        djTable = null;

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
        djTable.PlayLeftTrack(track.AudioClip);
    }

    public void SetRightTrack(DJTrack track)
    {
        rightTrack = track;
        djTable.PlayRightTrack(track.AudioClip);
    }

    public void SetTrackValue(float value)
    {
        leftValue = 1 - value;
        rightValue = value;
    }
}
