using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using UnityEngine;

public interface IMovementAbility
{
    public float Range { get; }
}

[CreateAssetMenu(menuName = "Fungals/Ability/Dash")]
public class FungalDash : DirectionalAbility, IMovementAbility
{
    [SerializeField] private float dashRange = 3f;
    [SerializeField] private float dashSpeed = 7.5f;

    [SerializeField] private List<AudioClip> dashAudio;

    private Movement Movement => fungal.Movement;
    private ClientNetworkTransform networkTransform;
    private AudioSource audioSource;

    public override float Range => dashRange;
    public override Vector3 DefaultTargetPosition => fungal.transform.position + fungal.transform.forward * dashRange;
    public override bool UseTrajectory => false;

    public override void Initialize(NetworkFungal fungal)
    {
        base.Initialize(fungal);

        networkTransform = fungal.GetComponent<ClientNetworkTransform>();
        audioSource = fungal.GetComponent<AudioSource>();
    }

    public override void CastAbility(Vector3 targetPosition)
    {
        base.CastAbility(targetPosition);

        void OnDestinationReached()
        {
            Movement.SetSpeed(fungal.BaseSpeed);
            Movement.OnDestinationReached -= OnDestinationReached;

            networkTransform.Interpolate = true;
            fungal.TrailRenderers.SetActive(false);
            CompleteAbility();
        }

        fungal.TrailRenderers.SetActive(true);
        networkTransform.Interpolate = false;
        Movement.OnDestinationReached += OnDestinationReached;

        Movement.SetSpeed(dashSpeed);
        Movement.SetTargetPosition(targetPosition);

        var audioClip = dashAudio.GetRandomItem();
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
