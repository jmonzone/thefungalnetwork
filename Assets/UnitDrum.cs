using UnityEngine;

public class UnitDrum : UnitBehaviour
{
    [Header("References")]
    [SerializeField] private DJTableReference dJTableReference;

    [Header("Settings")]
    [SerializeField] private int step;

    private PlantSporeEmitter plant;

    public void SetPlant(PlantSporeEmitter plant)
    {
        this.plant = plant;
    }

    public override void StartBehaviour()
    {
        dJTableReference.OnBeat += DJTableReference_OnBeat;
    }

    private void DJTableReference_OnBeat(int beat)
    {
        if (beat % step == 0) plant.EmitSpore();
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
        dJTableReference.OnBeat -= DJTableReference_OnBeat;
    }
}
