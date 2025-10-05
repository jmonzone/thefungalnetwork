using UnityEngine;
using UnityEngine.UI;

public class PTS_ActivityUI : ActivityUI<PTS_Unit, PTS_ActivityController>
{
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
        PlayerReference.TogglePOVCamera(true);
    }

    protected override void OnPlayerExit(ActivityUnit player)
    {
        base.OnPlayerExit(player);
        PlayerReference.TogglePOVCamera(false);
    }

    protected override void OnUnitSelected(PTS_Unit unit)
    {
        base.OnUnitSelected(unit);
        passButton.interactable = unit.IsPlayer;
    }


}
