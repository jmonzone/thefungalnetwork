using System.Collections.Generic;
using UnityEngine;

public class UnitDrum : UnitBehaviour
{
    [Header("References")]
    [SerializeField] private DJTableReference dJTableReference;

    [Header("Settings")]
    [SerializeField] private int step;
    [SerializeField] private List<AudioClip> audioClips;

    private PlantSporeEmitter plant;
    private Animator animator;
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void SetPlant(PlantSporeEmitter plant)
    {
        this.plant = plant;
    }

    protected override void OnBehaviourStart()
    {
        dJTableReference.OnBeat += DJTableReference_OnBeat;
    }

    private void DJTableReference_OnBeat(int beat)
    {
        if (beat % step == step - 1)
        {
            var randomIndex = Random.Range(0, audioClips.Count);
            audioSource.clip = audioClips[randomIndex];
            audioSource.PlayDelayed(0.1f);
            animator.SetTrigger("attack");
        }

        if (beat % step == 0)
        {
            plant.EmitSpore();
        }
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
        dJTableReference.OnBeat -= DJTableReference_OnBeat;
    }
}
