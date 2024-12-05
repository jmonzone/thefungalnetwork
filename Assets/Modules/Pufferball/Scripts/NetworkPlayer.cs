using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Events;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private Controllable player;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private Possession possesionService;
    [SerializeField] private FungalControllerSpawner fungalControllerSpawner;

    public Controllable Controllable { get; private set; }

    private NetworkTransform networkTransform;

    //todo: remove static events use scriptable object reference
    public static UnityAction<NetworkPlayer> OnLocalPlayerSpawned;
    public static UnityAction<NetworkPlayer> OnRemotePlayerSpawned;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            Debug.Log("init player");
            networkTransform = GetComponent<NetworkTransform>();

            // This is the local player
            Debug.Log("Local player spawned: " + gameObject.name);
            if (!TrySpawnPartner())
            {
                Controllable = player;
                player.gameObject.SetActive(true);
            }
            else
            {
                player.gameObject.SetActive(false);
            }

            Quaternion forwardRotation = Quaternion.LookRotation(Vector3.forward);

            var randomPosition = Random.insideUnitSphere.normalized;
            randomPosition.y = 0;

            var spawnPosition = arena.SpawnPosition1 + randomPosition;
            networkTransform.Teleport(spawnPosition, forwardRotation, Vector3.one);

            Debug.Log("event");
            OnLocalPlayerSpawned?.Invoke(this);
        }
        else
        {
            // This is a remote player
            Debug.Log("Remote player spawned: " + gameObject.name);
            player.gameObject.SetActive(true);
            //todo: spawn remote fungal if existing
            OnRemotePlayerSpawned?.Invoke(this);
        }
    }

    private bool TrySpawnPartner()
    {
        var partner = possesionService.Fungal;

        if (partner)
        {
            var targetFungal = fungalInventory.Fungals.Find(fungal => fungal.Data.Id == partner.Data.Id);

            if (targetFungal)
            {
                Debug.Log("target found");
                var fungalController = fungalControllerSpawner.SpawnFungal(targetFungal, transform.position);
                Controllable = fungalController.Controllable;
                return true;
            }

            Debug.Log("no target found");

        }

        return false;
    }
}
