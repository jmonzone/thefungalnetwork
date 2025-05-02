using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using UnityEngine;

public interface IMovementAbility
{
    public float Range { get; }
    public bool IgnoreObstacles { get; }
}

[CreateAssetMenu(menuName = "Fungals/Ability/Dash")]
public class FungalDash : DirectionalAbility, IMovementAbility
{
    [SerializeField] private float dashRange = 3f;
    [SerializeField] private float dashSpeed = 7.5f;

    [SerializeField] private List<AudioClip> dashAudio;

    private Movement Movement => Fungal.Movement;
    private ClientNetworkTransform networkTransform;
    private AudioSource audioSource;

    public override float Range => dashRange;
    public bool IgnoreObstacles => false;
    public override Vector3 DefaultTargetPosition => Fungal.transform.position + Fungal.transform.forward * dashRange;
    public override bool UseTrajectory => false;

    public override void Initialize(FungalController fungal, AbilitySlot index)
    {
        base.Initialize(fungal, index);

        networkTransform = fungal.GetComponent<ClientNetworkTransform>();
        audioSource = fungal.GetComponent<AudioSource>();
    }

    protected override void OnAbilityCasted(Vector3 targetPosition)
    {
        void OnDestinationReached()
        {
            Movement.SetSpeed(Fungal.BaseSpeed);
            Movement.OnDestinationReached -= OnDestinationReached;

            networkTransform.Interpolate = true;
            Fungal.ToggleTrailRenderers(false);
            CompleteAbility();
        }

        Fungal.ToggleTrailRenderers(true);
        networkTransform.Interpolate = false;
        Movement.OnDestinationReached += OnDestinationReached;

        Movement.SetSpeed(dashSpeed);
        Movement.SetTargetPosition(targetPosition);

        var audioClip = dashAudio.GetRandomItem();
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
