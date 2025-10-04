using System.Collections;
using UnityEngine;

public class PassTheSporePlayer : PassTheSporeUnit
{
    [SerializeField] private PassTheSporeUI passTheSporeUI;

    protected override IEnumerator WaitForPassInput()
    {
        yield return new WaitUntil(() => Input.GetMouseButton(0) && passTheSporeUI.IsGameplayUI);
    }
}
