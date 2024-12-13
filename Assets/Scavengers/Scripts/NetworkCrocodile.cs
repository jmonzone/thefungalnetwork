using Unity.Netcode;

public class NetworkCrocodile : NetworkBehaviour
{
    private CrocodileAttack attack;

    private void Awake()
    {
        attack = GetComponent<CrocodileAttack>();
        attack.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            attack.enabled = true;
        }
    }
}
