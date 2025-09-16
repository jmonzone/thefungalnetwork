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
    [SerializeField] private UnitListReference unitListReference;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference dialogueView;

    [Header("Runtime")]
    [SerializeField] private bool isActive;
    [SerializeField] private UnitController unit;
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private float experience;
    [SerializeField] private float relationship;

    public bool IsActive => isActive;
    public UnitController Unit => unit;
    public Dialogue Dialogue => dialogue;
    public float Experience => experience;
    public float Relationship => relationship;

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
        this.unit = unit;
        this.dialogue = dialogue;
        //Debug.Log($"ApplyStartDialogue {dialogue}");
        isActive = true;
        OnIsActiveChanged?.Invoke();

        experience = 0;
        relationship = 0;

        unit.Focus();
        if (navigation.CurrentView != dialogueView) navigation.Navigate(dialogueView);
    }

    public void RespondToChat(Response response)
    {
        Debug.Log($"RespondToChat {response.Next}");

        if (response.Next != null) dialogue = response.Next;
        experience += response.XP;
        relationship += response.Relationship;
        OnDialogueResponse?.Invoke(response);
    }

    public void ContinueDialogue()
    {
        Debug.Log($"ContinueDialogue {dialogue.Next.Action}");

        if (dialogue.Next != null) dialogue = dialogue.Next;
    }

    public void CompleteDialogue()
    {
        Debug.Log($"CompleteDialogue  {dialogue.Action}");

        unit.Unfocus();
        OnDialogueComplete?.Invoke();

        navigation.GoBackToRoot();

        unit = null;
        dialogue = null;
        isActive = false;
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
        StartDialogue(unit, unit.Instance.Data.ChatDialogue[0]);
    }

    public void StartGive()
    {
        inventory.OnItemSelected += Inventory_OnItemSelected;
        inventory.OpenInventory();
    }

    private void Inventory_OnItemSelected(Item arg0)
    {
        inventory.OnItemSelected -= Inventory_OnItemSelected;
        dialogue = unit.Instance.Data.GiveDialogue[0];
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
