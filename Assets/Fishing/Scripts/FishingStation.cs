using UnityEngine;

public class FishingStation : JobStation
{
    [SerializeField] private FishManager fishManager;
    [SerializeField] private FishingRod fishingRod;

    public override string ActionText => "Fish";

    private void Update()
    {
        if (IsActive && Input.GetMouseButtonDown(0) && !Utility.IsPointerOverUI) fishingRod.Use();
    }

    public override void SetFungal(FungalController fungal)
    {

    }

    protected override void OnJobStarted()
    {
        fishManager.enabled = true;
    }

    protected override void OnJobEnded()
    {
        fishManager.enabled = false;
    }

    protected override void OnBackButtonClicked()
    {
        EndAction();
    }
}
