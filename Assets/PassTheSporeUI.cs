using Cinemachine;
using UnityEngine;

public class PassTheSporeUI : ActivityUI
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    protected override void OnPlayerEnter(ActivityUnit player)
    {
        base.OnPlayerEnter(player);
        virtualCamera.Priority = 11;
    }

    protected override void OnPlayerExit(ActivityUnit player)
    {
        base.OnPlayerExit(player);
        virtualCamera.Priority = 0;
    }
}
