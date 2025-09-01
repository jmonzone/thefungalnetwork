using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DJTableReference : ScriptableObject
{
    [Header("Runtime")]
    [SerializeField] private float bpm = 90;
    [SerializeField] private bool isPlaying = false;

    public float BPM => bpm;
    public float BeatDuration => 60f / bpm; // seconds per beat
    public bool IsPlaying => isPlaying;

    public event UnityAction OnBPMChanged;
    public event UnityAction<int> OnBeat;

    public void Initialize()
    {
        bpm = 90;
        isPlaying = false;
    }

    public void SetIsPlaying(bool isPlaying)
    {
        this.isPlaying = isPlaying;
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
}
