using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DialogueReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private PhotoReference photoReference;
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference dialogueView;

    [Header("Runtime")]
    [SerializeField] private bool isActive;
    [SerializeField] private UnitController unit;
    [SerializeField] private Dialogue dialogue;

    public bool IsActive => isActive;
    public UnitController Unit => unit;
    public Dialogue Dialogue => dialogue;

    public event UnityAction OnIsActiveChanged;
    public event UnityAction OnInteractionStart;
    public event UnityAction OnDialogueStart;
    public event UnityAction<Response> OnDialogueResponse;
    public event UnityAction OnGiveComplete;
    public event UnityAction OnDialogueComplete;

    public void StartInteraction(UnitController unit, Dialogue dialogue)
    {
        ApplyStartDialogue(unit, dialogue);
        OnInteractionStart?.Invoke();
    }

    public void StartDialogue(UnitController unit, Dialogue dialogue)
    {
        ApplyStartDialogue(unit, dialogue);
        OnDialogueStart?.Invoke();
    }

    private void ApplyStartDialogue(UnitController unit, Dialogue dialogue)
    {
        isActive = true;
        this.unit = unit;
        this.dialogue = dialogue;
        unit.Focus();
        OnIsActiveChanged?.Invoke();
        if (navigation.CurrentView != dialogueView) navigation.Navigate(dialogueView);
    }

    public void RespondToChat(Response response)
    {
        dialogue = response.Next;
        OnDialogueResponse?.Invoke(response);
    }

    public void CompleteDialogue()
    {
        unit.Unfocus();
        OnDialogueComplete?.Invoke();
        navigation.GoBack();

        isActive = false;
        unit = null;
        dialogue = null;
        OnIsActiveChanged?.Invoke();
    }

    public void StartPhoto()
    {
        Unit.SetLookTarget(playerReference.Player.transform);
        photoReference.SetLookTarget(Unit.transform);
        photoReference.StartPhotoView();
    }

    public void StartChat()
    {
        StartDialogue(unit, unit.Data.ChatDialogue[0]);
    }

    public void StartGive()
    {
        inventory.OnItemSelected += Inventory_OnItemSelected;
        inventory.OpenInventory();
    }

    private void Inventory_OnItemSelected(Item arg0)
    {
        inventory.OnItemSelected -= Inventory_OnItemSelected;
        dialogue = unit.Data.GiveDialogue[0];
        OnGiveComplete?.Invoke();
        navigation.GoBack();
    }

    public void StartFollow()
    {
        if (Unit is FungalController fungal)
        {
            fungal.SetTarget(playerReference.Player.transform);
            CompleteDialogue();
        }
    }
}
