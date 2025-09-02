using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DJTableReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private BuildReference buildReference;

    [Header("Runtime")]
    [SerializeField] private DJTableController djTable;
    [SerializeField] private DJTrack leftTrack;
    [SerializeField] private DJTrack rightTrack;
    [SerializeField] private float bpm = 90;
    [SerializeField] private List<PlantSporeEmitter> plants;

    public DJTableController DjTable => djTable;
    public DJTrack LeftTrack => leftTrack;
    public DJTrack RightTrack => rightTrack;

    public float BPM => bpm;
    public float BeatDuration => 60f / bpm; // seconds per beat
    public List<PlantSporeEmitter> Plants => plants;

    public event UnityAction OnBPMChanged;
    public event UnityAction<int> OnBeat;

    public void Initialize()
    {
        bpm = 90;
        plants = new List<PlantSporeEmitter>();
        djTable = null;
        buildReference.OnBuildUpdated += BuildReference_OnBuildUpdated;
    }

    private void BuildReference_OnBuildUpdated()
    {
        plants = new List<PlantSporeEmitter>();
        foreach(var build in buildReference.BuildControllers)
        {
            var plant = build.GetComponent<PlantSporeEmitter>();
            if (plant) plants.Add(plant);
        }
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
}
