using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DJTableReference : ScriptableObject
{
    [SerializeField] private float bpm = 135f;

    public float BPM => bpm;

    public event UnityAction OnBPMChanged;

    public void SetBPM(float bpm)
    {
        this.bpm = bpm;
        OnBPMChanged?.Invoke();
    }
}
