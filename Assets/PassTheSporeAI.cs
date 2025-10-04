using System.Collections;
using UnityEngine;

public class PassTheSporeAI : PassTheSporeUnit
{
    [SerializeField] private DJTableReference djReference;

    protected override IEnumerator WaitForPassInput()
    {
        yield return new WaitForSeconds(djReference.BeatDuration * 1f);
    }
}