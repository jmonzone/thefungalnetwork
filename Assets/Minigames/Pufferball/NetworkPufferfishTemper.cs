using Unity.Netcode;

public class NetworkPufferfishTemper : NetworkBehaviour
{
    private PufferfishTemper pufferfishTemper;

    public NetworkVariable<float> Temper = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        pufferfishTemper = GetComponent<PufferfishTemper>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Ensure the server owns the state at the start
            Temper.Value = pufferfishTemper.Temper;
        }

        Temper.OnValueChanged += (oldValue, newValue) => pufferfishTemper.SetTemper(newValue);
    }

    public override void OnNetworkDespawn()
    {
        Temper.OnValueChanged -= (oldValue, newValue) => pufferfishTemper.SetTemper(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartTemperServerRpc()
    {
        pufferfishTemper.StartTemper();
    }

    [ServerRpc(RequireOwnership = false)]
    public void StopTimerServerRpc()
    {
        pufferfishTemper.StopTimer();
        Temper.Value = 0f;
    }

    private void Update()
    {
        if (IsServer)
        {
            Temper.Value = pufferfishTemper.Temper; // Sync Temper value from server
        }
    }
}
