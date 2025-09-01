using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DJTableReference : ScriptableObject
{
    [SerializeField] private float bpm = 135f;

    public float BPM => bpm;
    public float BeatDuration => 60f / bpm; // seconds per beat

    public event UnityAction OnBPMChanged;
    public event UnityAction OnBeat;

    public void SetBPM(float bpm)
    {
        this.bpm = bpm;
        OnBPMChanged?.Invoke();
    }

    public void InvokeBeat()
    {
        OnBeat?.Invoke();
    }
}
