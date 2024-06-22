using UnityEngine;

public enum FungalFishState
{
    IDLE,
    FISHING
}

public class FishingStation : JobStation
{
    [SerializeField] private FishManager fishManager;
    [SerializeField] private FishingRod fishingRod;

    private FungalController fungal;
    private float fishingTimer;
    private float baseDistanceThreshold;
    private FungalFishState state;

    public override void SetFungal(FungalController fungal)
    {
        this.fungal = fungal;
        SetFungalState(FungalFishState.IDLE);
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
        fungal.Movement.SetDistanceThreshold(baseDistanceThreshold);
    }

    protected override void OnBackButtonClicked()
    {
        EndAction();
    }

    private void SetFungalState(FungalFishState state)
    {
        this.state = state;
        switch (state)
        {
            case FungalFishState.IDLE:
                fishingTimer = 0;

                var origin = fishManager.transform.position;
                if (fungal.Model.Data.Type == FungalType.SKY) origin += Vector3.up * 1.5f;
                baseDistanceThreshold = fungal.Movement.DistanceThreshold;
                fungal.Movement.StartRadialMovement(origin);
                break;
        }
    }

    private void Update()
    {

        switch (state)
        {
            case FungalFishState.IDLE:
                fishingTimer += Time.deltaTime;
                if (fishingTimer > 5f) SetFungalState(FungalFishState.FISHING);
                break;
            case FungalFishState.FISHING:
                var catchableFish = fungal.transform.OverlapSphere<FishController>(20f, fish =>
                {
                    return fish.State == FishState.IDLE;
                });

                if (catchableFish.Count > 0)
                {
                    fungal.Movement.SetDistanceThreshold(0.1f);
                    fungal.Movement.SetTarget(catchableFish[0].transform);
                }

                if (fungal.Movement.IsAtDestination) SetFungalState(FungalFishState.IDLE);
                break;
        }
    }
}
