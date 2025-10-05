using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PTS_ActivityUI : ActivityUI<PTS_Unit, PTS_ActivityController>
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Button passButton;

    protected override void Awake()
    {
        base.Awake();

        passButton.onClick.AddListener(() =>
        {
            Controller.PassSpore();
        });
    }

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

    protected override void OnUnitSelected(PTS_Unit unit)
    {
        base.OnUnitSelected(unit);
        passButton.interactable = unit.IsPlayer;
    }


}
