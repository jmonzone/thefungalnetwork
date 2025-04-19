using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Fish : NetworkBehaviour
{
    [SerializeField] private GameReference pufferballReference;

    [SerializeField] private Sprite icon;
    [SerializeField] private string abilityName;
    [SerializeField] private Color backgroundColor;

    [SerializeField] private float audioPitch;
    [SerializeField] private List<AudioClip> audioClips;

    public Sprite Icon => icon;
    public string AbilityName => abilityName;
    public Color BackgroundColor => backgroundColor;

    public Movement Movement { get; private set; }

    private AudioSource audioSource;

    public NetworkVariable<ulong> PickedUpFungalId = new NetworkVariable<ulong>();
    public NetworkVariable<bool> IsPickedUp = new NetworkVariable<bool>(false);

    public event UnityAction OnPickup;
    public event UnityAction OnRespawn;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Movement = GetComponent<Movement>();
        audioSource = GetComponent<AudioSource>();

        var fishController = GetComponent<FishController>();
        if (IsOwner) fishController.OnRespawnComplete += OnRespawnServerRpc;
    }

    public void Catch(Transform bobber)
    {
        Movement.SetSpeed(10);
        Movement.Follow(bobber);
        RequestCatchServerRpc(NetworkManager.Singleton.LocalClientId);
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
                return;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnRespawnServerRpc()
    {
        IsPickedUp.Value = false;
    }

}