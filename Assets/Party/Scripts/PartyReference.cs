using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class PartyReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference partyListView;
    [SerializeField] private ViewReference partyHUD;
    [SerializeField] private ViewReference debriefView;
    [SerializeField] private ViewReference gameplayView;
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
    public event UnityAction OnPartyDebriefComplete;
    public event UnityAction OnPartyResumed;
    public event UnityAction OnScoreChanged;

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
        if (isActive)
        {
            score += value;
            OnScoreChanged?.Invoke();
        }
    }

    public void ShowPartyList()
    {
        navigation.Navigate(partyListView);
    }

    public void StartParty(PartyData party)
    {
        Debug.Log("Starting the party");

        score = 0;
        OnScoreChanged?.Invoke();
        sporesCollected = 0;
        isActive = true;
        currentParty = party;

        //navigation.Navigate(partyHUD);
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
        if (!isActive) return;

        Debug.Log("Stopping the party");
        foreach (var guest in guests)
        {
            guest.gameObject.SetActive(false);
        }

        guests = new List<UnitController>();

        navigation.Navigate(debriefView);
        OnPartyComplete?.Invoke();
    }

    public void CompleteDebrief()
    {
        navigation.Navigate(gameplayView);
        OnPartyDebriefComplete?.Invoke();
        isActive = false;
        currentParty = null;
    }

    public void AddGuest(UnitController guest)
    {
        guests.Add(guest);
    }
}
