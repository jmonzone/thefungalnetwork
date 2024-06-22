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
    [SerializeField] private TextPopup textPopup;

    private float fishingTimer;
    private float baseDistanceThreshold;
    private FungalFishState state;
    private FishController targetFish;

    protected override void Awake()
    {
        base.Awake();
        fishingRod.OnStateChanged += state =>
        {
            if (state == FishingRodState.REELING && fishingRod.TargetFish)
            {
                OnFishCaught(fishingRod.TargetFish);
            }
        };
    }

    protected override void OnFungalChanged(FungalController fungal)
    {
        SetFungalState(FungalFishState.IDLE);
        fungal.Movement.SetDistanceThreshold(0.1f);
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
        Fungal.Movement.SetDistanceThreshold(baseDistanceThreshold);
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
                if (Fungal.Model.Data.Type == FungalType.SKY) origin += Vector3.up * 1.5f;
                baseDistanceThreshold = Fungal.Movement.DistanceThreshold;
                Fungal.Movement.StartRadialMovement(origin);
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
                var catchableFish = Fungal.transform.OverlapSphere<FishController>(20f, fish =>
                {
                    return fish.State == FishState.IDLE;
                });

                if (catchableFish.Count > 0)
                {
                    targetFish = catchableFish[0];
                    Fungal.Movement.SetTarget(targetFish.transform);

                    if (Fungal.Movement.IsAtDestination)
                    {
                        OnFishCaught(targetFish);
                        targetFish.gameObject.SetActive(false);
                        targetFish = null;
                        SetFungalState(FungalFishState.IDLE);
                    }
                }
                break;
        }
    }

    private void OnFishCaught(FishController fish)
    {
        var position = Camera.main.WorldToScreenPoint(fish.transform.position);
        textPopup.transform.position = position;
        textPopup.ShowText($"+{fish.Data.Experience}");
        Fungal.Model.Experience += fish.Data.Experience;
    }
}
