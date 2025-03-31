using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Fish : NetworkBehaviour
{
    [SerializeField] private PufferballReference pufferballReference;
    [SerializeField] private float swimSpeed = 1f;
    [SerializeField] private bool useTrajectory = false;

    [SerializeField] private Sprite icon;
    [SerializeField] private string abilityName;
    [SerializeField] private Color backgroundColor;
    [SerializeField] private float score;

    [SerializeField] private float audioPitch;
    [SerializeField] private List<AudioClip> audioClips;

    public Sprite Icon => icon;
    public string AbilityName => abilityName;
    public Color BackgroundColor => backgroundColor;
    public float Score => score;
    public bool UseTrajectory => useTrajectory;

    public Movement Movement { get; private set; }
    public ThrowFish ThrowFish { get; private set; }

    private AudioSource audioSource;


    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(
        Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public NetworkVariable<ulong> PickedUpFungalId = new NetworkVariable<ulong>();
    public NetworkVariable<bool> IsPickedUp = new NetworkVariable<bool>(false);

    public event UnityAction<bool> OnPickUpRequest;
    public event UnityAction OnPrepareThrow;
    public event UnityAction OnPickup;
    public event UnityAction OnRespawn;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Movement = GetComponent<Movement>();
        audioSource = GetComponent<AudioSource>();
        ThrowFish = GetComponent<ThrowFish>();

        if (IsServer)
        {
            networkPosition.Value = transform.position; // Set initial position on the server
        }
    }

    public void Catch(Transform bobber)
    {
        Movement.SetSpeed(10);
        Movement.Follow(bobber);
        RequestCatchServerRpc(NetworkManager.Singleton.LocalClientId);
    }


    public void PrepareThrow()
    {
        OnPrepareThrow?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestCatchServerRpc(ulong requestingClientId)
    {
        NetworkObject.ChangeOwnership(requestingClientId);
    }

    private bool requested;

    [ServerRpc(RequireOwnership = false)]
    public void RequestPickUpServerRpc(ulong requestingObjectId)
    {
        //Debug.Log($"RequestPickUpServerRpc requestingObjectId: {requestingObjectId}");
        if (requested)
        {
            OnRequestPickUpClientRpc(requestingObjectId, false);
            return;
        }

        requested = true;

        if (!IsPickedUp.Value)
        {
            IsPickedUp.Value = true;
            PickedUpFungalId.Value = requestingObjectId;

            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(requestingObjectId, out var networkObject))
            {
                NetworkObject.ChangeOwnership(networkObject.OwnerClientId);
            }

            OnRequestPickUpClientRpc(requestingObjectId, true);
        }
        else
        {
            OnRequestPickUpClientRpc(requestingObjectId, false);
        }

    }


    [ClientRpc]
    private void OnRequestPickUpClientRpc(ulong requestingObjectId, bool success)
    {
        //Debug.Log($"OnRequestPickUpClientRpc requestingObjectId: {requestingObjectId}");

        if (IsServer) requested = false;

        //Debug.Log($"OnPickupClientRpc {NetworkManager.Singleton.LocalClientId == clientId}");
        if (success)
        {
            audioSource.clip = audioClips.GetRandomItem();
            audioSource.pitch = audioPitch;
            audioSource.Play();

            if (IsOwner && NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(requestingObjectId, out var networkObject))
            {
                Movement.SetSpeed(10);
                Movement.Follow(networkObject.transform);
                OnPickup?.Invoke();  // Trigger pickup event only on the owning client
                OnPickUpRequest?.Invoke(true);
                return;
            }
        }

        OnPickUpRequest?.Invoke(false);
    }

    public void ReturnToRadialMovement()
    {
        Debug.Log($"ReturnToRadialMovement {name}");
        Movement.SetSpeed(swimSpeed);
        Movement.StartRadialMovement(networkPosition.Value, true);
        OnRespawnServerRpc();
        OnRespawn?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnRespawnServerRpc()
    {
        IsPickedUp.Value = false;
    }

}