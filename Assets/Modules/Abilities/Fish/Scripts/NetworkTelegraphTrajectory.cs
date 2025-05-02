using Unity.Netcode;
using UnityEngine;

public class NetworkTelegraphTrajectory : NetworkBehaviour
{
    private TelegraphTrajectory telegraphTrajectory;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        telegraphTrajectory = GetComponent<TelegraphTrajectory>();

        telegraphTrajectory.OnIndicatorShow += (position, radius) =>
        {
            if (IsOwner) HandleShowIndicatorServerRpc(position, radius);
        };

        telegraphTrajectory.OnIndicatorHide += () =>
        {
            if (IsOwner) HandleHideIndicatorServerRpc();
        };
    }

    [ServerRpc]
    private void HandleShowIndicatorServerRpc(Vector3 targetPosition, float radius)
    {
        HandleShowIndicatorClientRpc(targetPosition, radius);
    }

    [ClientRpc]
    private void HandleShowIndicatorClientRpc(Vector3 targetPosition, float radius)
    {
        if (!IsOwner)
        {
            telegraphTrajectory.ShowIndicator(targetPosition, radius);
        }
    }

    [ServerRpc]
    private void HandleHideIndicatorServerRpc()
    {
        HandleHideIndicatorClientRpc();
    }

    [ClientRpc]
    private void HandleHideIndicatorClientRpc()
    {
        if (!IsOwner)
        {
            telegraphTrajectory.HideIndicator();
        }
    }
}
