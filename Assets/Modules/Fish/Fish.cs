using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Fish : NetworkBehaviour
{
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private float swimSpeed = 1f;

    [SerializeField] private Sprite icon;
    [SerializeField] private string abilityName;
    [SerializeField] private Color backgroundColor;

    public Sprite Icon => icon;
    public string AbilityName => abilityName;
    public Color BackgroundColor => backgroundColor;

    public Movement Movement { get; private set; }
    private ThrowFish throwFish;

    [SerializeField] private float audioPitch;
    [SerializeField] private List<AudioClip> audioClips;
    private AudioSource audioSource;

    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(
        Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public NetworkVariable<bool> IsPickedUp = new NetworkVariable<bool>(false);

    public event UnityAction<bool> OnPickUpRequest;
    public event UnityAction OnPrepareThrow;
    public event UnityAction OnPickup;
    public event UnityAction OnRespawn;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Movement = GetComponent<Movement>();
        throwFish = GetComponent<ThrowFish>();
        audioSource = GetComponent<AudioSource>();

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

    public void Throw(Vector3 targetPosition)
    {
        throwFish.Throw(targetPosition);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestCatchServerRpc(ulong requestingClientId)
    {
        NetworkObject.ChangeOwnership(requestingClientId);
    }

    private bool requested;

    [ServerRpc(RequireOwnership = false)]
    public void RequestPickUpServerRpc(ulong clientId)
    {
        if (requested)
        {
            OnRequestPickUpClientRpc(clientId, false);
            return;
        }

        requested = true;

        if (!IsPickedUp.Value)
        {
            IsPickedUp.Value = true;
            NetworkObject.ChangeOwnership(clientId);
            OnRequestPickUpClientRpc(clientId, true);
        }
        else
        {
            OnRequestPickUpClientRpc(clientId, false);
        }

    }


    [ClientRpc]
    private void OnRequestPickUpClientRpc(ulong clientId, bool success)
    {
        if (IsServer) requested = false;

        //Debug.Log($"OnPickupClientRpc {NetworkManager.Singleton.LocalClientId == clientId}");
        if (success)
        {
            // Update the local state after ownership change
            if (NetworkManager.Singleton.LocalClientId == clientId)
            {
                audioSource.clip = audioClips.GetRandomItem();
                audioSource.pitch = audioPitch;
                audioSource.Play();
                Movement.SetSpeed(10);
                Movement.Follow(playerReference.Movement.transform);
                OnPickup?.Invoke();  // Trigger pickup event only on the owning client
                OnPickUpRequest?.Invoke(true);
                return;
            }
        }

        OnPickUpRequest?.Invoke(false);
    }

    public void ReturnToRadialMovement()
    {
        //Debug.Log("ReturnToRadialMovement");
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