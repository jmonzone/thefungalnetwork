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
    [SerializeField] private Dialogue currentDialogue;
    [SerializeField] private List<Dialogue> dialogue;

    public bool IsActive => isActive;
    public UnitController Unit => unit;
    public Dialogue CurrentDialogue => currentDialogue;
    public List<Dialogue> Dialogue => dialogue;


    //todo: consolidate events;
    public event UnityAction OnIsActiveChanged;

    public event UnityAction OnDialogueStart;
    public event UnityAction OnDialogueComplete;

    public event UnityAction OnChatStart;

    public event UnityAction OnSpecialDialogueStart;

    public event UnityAction OnGiveComplete;
    public event UnityAction<Response> OnChatResponded;


    public void StartDialogue(UnitController unit, List<Dialogue> dialogue)
    {
        ApplyStartDialogue(unit, dialogue);
        OnDialogueStart?.Invoke();
    }

    public void SetCurrentDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
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

    public void StartChat()
    {
        dialogue = unit.Data.ChatDialogue;
        OnChatStart?.Invoke();
    }

    public void StartSpecialDialogue(UnitController unit, List<Dialogue> dialogue)
    {
        ApplyStartDialogue(unit, dialogue);
        OnSpecialDialogueStart?.Invoke();
    }

    private void ApplyStartDialogue(UnitController unit, List<Dialogue> dialogue)
    {
        isActive = true;
        this.unit = unit;
        this.dialogue = dialogue;
        unit.Focus();
        OnIsActiveChanged?.Invoke();
        navigation.Navigate(dialogueView);
    }

    public void StartPhoto()
    {
        Unit.SetLookTarget(playerReference.Player.transform);
        photoReference.SetLookTarget(Unit.transform);
        photoReference.StartPhotoView();
    }

    public void StartGive()
    {
        inventory.OnItemSelected += Inventory_OnItemSelected;
        inventory.OpenInventory();
    }

    private void Inventory_OnItemSelected(Item arg0)
    {
        inventory.OnItemSelected -= Inventory_OnItemSelected;
        dialogue = unit.Data.GiveDialogue;
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

    public void RespondToChat(Response response)
    {
        OnChatResponded?.Invoke(response);
    }
}
