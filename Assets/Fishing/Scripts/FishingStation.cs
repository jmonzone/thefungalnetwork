using UnityEngine;

public class FishingStation : JobStation
{
    [SerializeField] private FishManager fishManager;
    [SerializeField] private FishingRod fishingRod;

    public override void SetFungal(FungalController fungal)
    {

    }

    protected override void OnJobStarted()
    {
        fishManager.enabled = true;
    }

    protected override void OnCameraPrepared()
    {
        fishingRod.gameObject.SetActive(true);
    }

    protected override void OnJobEnded()
    {
        fishManager.enabled = false;
        fishingRod.gameObject.SetActive(false);
    }

    protected override void OnBackButtonClicked()
    {
        EndAction();
    }
}
