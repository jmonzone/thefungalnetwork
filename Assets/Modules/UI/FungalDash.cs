using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using UnityEngine;
using UnityEngine.Events;

public class FungalDash : Ability
{
    [SerializeField] private float dashRange = 3f;
    [SerializeField] private float dashSpeed = 7.5f;

    [SerializeField] private List<AudioClip> dashAudio;
    [SerializeField] private GameObject trailRenderers;

    private Movement movement;
    private NetworkFungal fungal;
    private ClientNetworkTransform networkTransform;
    private AudioSource audioSource;

    public override float Range => dashRange;
    public override Vector3 DefaultTargetPosition => transform.position + transform.forward * dashRange;
    public override bool UseTrajectory => false;

    public event UnityAction OnDashStart;
    public event UnityAction OnDashComplete;

    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<Movement>();
        fungal = GetComponent<NetworkFungal>();
        networkTransform = GetComponent<ClientNetworkTransform>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void CastAbility(Vector3 targetPosition)
    {
        OnDashStart?.Invoke();

        void OnDestinationReached()
        {
            movement.SetSpeed(fungal.BaseSpeed);
            movement.OnDestinationReached -= OnDestinationReached;

            networkTransform.Interpolate = true;
            trailRenderers.SetActive(false);
            OnDashComplete?.Invoke();
        }

        trailRenderers.SetActive(true);
        networkTransform.Interpolate = false;
        movement.OnDestinationReached += OnDestinationReached;

        movement.SetSpeed(dashSpeed);
        movement.SetTargetPosition(targetPosition);

        var audioClip = dashAudio.GetRandomItem();
        audioSource.clip = audioClip;
        audioSource.Play();

        StartCoroutine(Cooldown.StartCooldown());
    }
}
