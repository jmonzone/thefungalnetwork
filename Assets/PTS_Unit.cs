using UnityEngine;
using UnityEngine.Events;

public class PTS_Unit : ActivityBehaviour
{
    public Vector3 SporePosition => transform.position + Vector3.up;

    private PTS_SporeController spore;

    protected override void OnBehaviourStart()
    {

    }

    public void GiveSpore(PTS_SporeController spore)
    {
        this.spore = spore;
    }

    public void PassSpore(PTS_Unit target, UnityAction<PTS_Unit> onComplete)
    {
        IncreaseXP(100);
        spore.Pass(target, () => onComplete?.Invoke(target));
        spore = null;
    }

    protected override void Update()
    {
        base.Update();
        if (spore)
        {
            spore.transform.position = SporePosition;
        }
    }
}
