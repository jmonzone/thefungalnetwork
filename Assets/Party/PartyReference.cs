using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PartyPhase
{
    DOORS_OPEN = 0,
    COCKTAIL_HOUR = 1,
    EVENT = 2,
    WIND_DOWN = 3,
    CLEANUP = 4
}

[CreateAssetMenu]
public class PartyReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference partyHUD;
    [SerializeField] private ViewReference debriefView;
    [SerializeField] private UnitListReference unitList;
    [SerializeField] private DialogueReference dialogueReference;
    [SerializeField] private PhotoReference photoReference;
    [SerializeField] private InventoryReference inventoryReference;

    [Header("Data")]
    [SerializeField] private List<PartyData> parties;

    [Header("Runtime")]
    [SerializeField] private bool isActive;
    [SerializeField] private PartyData currentParty;
    [SerializeField] private int score;
    [SerializeField] private int sporesCollected;
    [SerializeField] private List<UnitController> guests;

    public List<PartyData> Parties => parties;

    public bool IsActive => isActive;
    public PartyData CurrentParty => currentParty;
    public int Score => score;
    public int SporesCollected => sporesCollected;
    public List<UnitController> Guests => guests;

    public event UnityAction OnPartyStarted;
    public event UnityAction OnPartyPaused;
    public event UnityAction OnPartyComplete;
    public event UnityAction OnPartyResumed;

    public void Initialize()
    {
        currentParty = null;
        isActive = false;
        guests = new List<UnitController>();
        dialogueReference.OnDialogueComplete += DialogueReference_OnDialogueComplete;
        photoReference.OnPhotoTaken += PhotoReference_OnPhotoTaken;
        inventoryReference.OnSporeCollected += InventoryReference_OnSporeCollected;
    }

    private void InventoryReference_OnSporeCollected(SporeController arg0)
    {
        sporesCollected += 1;
        IncrementScore(1);
    }

    private void PhotoReference_OnPhotoTaken()
    {
        IncrementScore(25);
    }

    private void DialogueReference_OnDialogueComplete()
    {
        IncrementScore(15);
    }

    private void IncrementScore(int value)
    {
        if (isActive) score += value;
    }

    public void StartParty(PartyData party)
    {
        score = 0;
        sporesCollected = 0;
        isActive = true;
        currentParty = party;

        navigation.Navigate(partyHUD);
        OnPartyStarted?.Invoke();
    }

    public void PauseParty()
    {
        isActive = false;
        OnPartyPaused?.Invoke();
    }

    public void ResumeParty()
    {
        isActive = true;
        OnPartyResumed?.Invoke();
    }

    public void StopParty()
    {
        navigation.Navigate(debriefView);
        OnPartyComplete?.Invoke();
        isActive = false;
        currentParty = null;
    }

    public void AddGuest(UnitController guest)
    {
        guests.Add(guest);
    }

    public void ClearGuests()
    {
        foreach (var guest in guests)
        {
            guest.gameObject.SetActive(false);
        }

        guests = new List<UnitController>();
    }
}
