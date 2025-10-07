using UnityEngine;
using UnityEngine.Events;

public class PTS_Unit : ActivityBehaviour
{
    [SerializeField] private Vector3 sporeOffset;
    public Vector3 SporePosition => transform.position + LookRotation * sporeOffset;

    private PTS_SporeController spore;

    protected override void OnBehaviourStart()
    {

    }

    public override void OnUnselect()
    {
        base.OnUnselect();
    }

    public void GiveSpore(PTS_SporeController spore)
    {
        this.spore = spore;
    }

    public void PassSpore(PTS_Unit target, UnityAction<PTS_Unit> onComplete)
    {
        spore.Pass(target, () => onComplete?.Invoke(target));
        spore = null;
    }

    public void CollectSpore(Vector3 source)
    {
        IncreaseXP(1, source);
    }

    protected override void Update()
    {
        base.Update();
        if (spore)
        {
            float lerpSpeed = 5f; // tweak for how fast it follows
            spore.transform.position = Vector3.Lerp(spore.transform.position, SporePosition, Time.deltaTime * lerpSpeed);
        }
    }
}
