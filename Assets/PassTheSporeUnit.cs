using System.Collections;
using UnityEngine;

public abstract class PassTheSporeUnit : ActivityBehaviour
{
    private Transform spore;

    public Vector3 SporePosition => transform.position + Vector3.up;

    protected override void OnBehaviourStart()
    {

    }

    public virtual IEnumerator PassRoutine()
    {
        yield return WaitForPassInput();
        IncreaseXP(100);
        spore = null;
    }

    protected abstract IEnumerator WaitForPassInput();

    public void GiveSpore(Transform spore)
    {
        this.spore = spore;
        StartCoroutine(GiveRoutine());
    }

    private IEnumerator GiveRoutine()
    {
        while (spore)
        {
            spore.transform.position = SporePosition;
            yield return new WaitForFixedUpdate();
        }
    }
}
