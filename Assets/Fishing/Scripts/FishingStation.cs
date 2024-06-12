using UnityEngine;

public class FishingStation : JobStation
{
    public override string ActionText => "Fish";

    protected override void OnBackButtonClicked()
    {
        EndAction();
    }

    protected override void OnJobStarted()
    {
    }
}
