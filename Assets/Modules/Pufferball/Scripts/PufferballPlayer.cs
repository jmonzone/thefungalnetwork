using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Events;

public class PufferballPlayer : NetworkBehaviour, IControllable
{
    [SerializeField] private MultiplayerArena arena;
    [SerializeField] private GameObject player;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private Possession possesionService;
    [SerializeField] private ProximityInteraction proximityInteraction;

    public MovementController Movement { get; private set; }
    public ProximityInteraction Interactions => proximityInteraction;

    private NetworkTransform networkTransform;

    //todo: remove static events use scriptable object reference
    public static UnityAction<PufferballPlayer> OnLocalPlayerSpawned;
    public static UnityAction<PufferballPlayer> OnRemotePlayerSpawned;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Movement = GetComponent<MovementController>();

        if (IsOwner)
        {
            Debug.Log("init player");
            networkTransform = GetComponent<NetworkTransform>();

            // This is the local player
            Debug.Log("Local player spawned: " + gameObject.name);
            if (!TrySpawnPartner()) SetRender(player);

            Quaternion forwardRotation = Quaternion.LookRotation(Vector3.forward);

            var randomPosition = Random.insideUnitSphere.normalized;
            randomPosition.y = 0;

            var spawnPosition = arena.SpawnPosition1 + randomPosition;
            networkTransform.Teleport(spawnPosition, forwardRotation, Vector3.one);

            OnLocalPlayerSpawned?.Invoke(this);
        }
        else
        {
            // This is a remote player
            Debug.Log("Remote player spawned: " + gameObject.name);
            SetRender(player);
            OnRemotePlayerSpawned?.Invoke(this);
        }
    }

    private bool TrySpawnPartner()
    {
        var partner = possesionService.Fungal;

        if (partner)
        {
            var targetFungal = fungalCollection.Data.Find(fungal => fungal.Id == partner.Data.Id);

            if (targetFungal)
            {
                Debug.Log("target found");
                var render = Instantiate(targetFungal.Prefab, transform);
                SetRender(render);
                return true;
            }

            Debug.Log("no target found");

        }

        return false;
    }

    private void SetRender(GameObject render)
    {
        render.SetActive(true);

        var animator = render.GetComponent<Animator>();

        var movementAnimations = GetComponent<MovementAnimations>();
        movementAnimations.SetAnimatior(animator);

        var ownerNetworkAnimator = render.GetComponent<OwnerNetworkAnimator>();
        ownerNetworkAnimator.Animator = animator;
    }
}
