using UnityEngine;

public class FishingStation : JobStation
{
    [SerializeField] private FishManager fishManager;
    [SerializeField] private FishingRod fishingRod;

    public override string ActionText => "Fish";

    public override void SetFungal(FungalController fungal)
    {

    }

    protected override void OnJobStarted()
    {
        fishManager.enabled = true;
        fishingRod.enabled = true;
    }

    protected override void OnJobEnded()
    {
        fishManager.enabled = false;
        fishingRod.enabled = false;
    }

    protected override void OnBackButtonClicked()
    {
        EndAction();
    }
}
