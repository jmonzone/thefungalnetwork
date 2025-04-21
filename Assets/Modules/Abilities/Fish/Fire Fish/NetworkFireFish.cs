using Unity.Netcode;

public class NetworkFireFish : NetworkBehaviour
{
    private FireFish fireFish;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        fireFish = GetComponent<FireFish>();

        var fish = GetComponent<FishController>();
        fish.ThrowFish.OnThrowComplete += OnThrowCompleteServerRpc;
    }

    [ServerRpc]
    private void OnThrowCompleteServerRpc()
    {
        OnThrowCompleteClientRpc();
    }

    [ClientRpc]
    private void OnThrowCompleteClientRpc()
    {
        if (!IsOwner) fireFish.ShowExplosionAnimation();
    }
}