using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitParty : MonoBehaviour
{
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private float energy;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        var dialogue = GetComponent<UnitDialogue>();
        dialogue.OnDialogueComplete += Dialogue_OnDialogueComplete;
    }

    private void Dialogue_OnDialogueComplete()
    {
        IncrementEnergy(20f);
    }

    private void OnEnable()
    {
        partyReference.OnPartyStarted += PartyReference_OnPartyStarted;
    }

    private void OnDisable()
    {
        partyReference.OnPartyStarted -= PartyReference_OnPartyStarted;
    }

    private void PartyReference_OnPartyStarted()
    {
        energy = 70f;
    }

    private void IncrementEnergy(float value)
    {
        energy += value;

        animator.SetFloat("Energy", energy);
    }
}
