using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Events;

public class PufferballPlayer : NetworkBehaviour, IControllable
{
    [SerializeField] private GameObject player;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private PufferballController pufferballPrefab;
    [SerializeField] private PufferballController pufferball;

    public bool HasPufferball { get; private set; }

    public MovementController Movement { get; private set; }

    private NetworkTransform networkTransform;

    public static UnityAction<PufferballPlayer> OnLocalPlayerSpawned;
    public static UnityAction<PufferballPlayer> OnRemotePlayerSpawned;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Movement = GetComponent<MovementController>();

        if (IsOwner)
        {
            Debug.Log("initifgh");
            networkTransform = GetComponent<NetworkTransform>();

            var detectCollider = GetComponent<DetectCollider>();
            detectCollider.OnColliderDetected += collider =>
            {
                Debug.Log($"HasPufferball {HasPufferball}");

                if (!HasPufferball)
                {
                    pufferball = collider.GetComponentInParent<PufferballController>();
                    CatchBall();
                }
            };

            // This is the local player
            Debug.Log("Local player spawned: " + gameObject.name);
            if (!TrySpawnPartner()) SetRender(player);

            if (IsHost)
            {
                var spawnPosition = new Vector3(0 , 2, 0);
                // Instantiate the object only on the server
                pufferball = Instantiate(pufferballPrefab, spawnPosition, Quaternion.identity);
                // Spawn the object across the network
                pufferball.GetComponent<NetworkObject>().Spawn();

                Quaternion forwardRotation = Quaternion.LookRotation(Vector3.forward);

                networkTransform.Teleport(new Vector3(0, 2, -4), forwardRotation, Vector3.one);
            }
            else
            {
                Quaternion backRotation = Quaternion.LookRotation(Vector3.back);

                networkTransform.Teleport(new Vector3(0, 2, 4), backRotation, Vector3.one);
            }



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

    private void Update()
    {
        if (HasPufferball && IsOwner)
        {
            var targetPosition = transform.position + Vector3.up * 2;
            pufferball.MoveObjectServerRpc(targetPosition);
        }
    }

    private void CatchBall()
    {
        HasPufferball = true;
        pufferball.TogglePhysicsServerRpc(false);
    }

    public void LaunchBall()
    {
        HasPufferball = false;
        pufferball.LaunchServerRpc(transform.forward);
    }

    private bool TrySpawnPartner()
    {
        var partner = GameManager.Instance.GetPartner();

        if (partner)
        {
            var targetFungal = fungalCollection.Data.Find(fungal => fungal.Id == partner?.Data.Id);

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
