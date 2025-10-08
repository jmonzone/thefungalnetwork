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
    [SerializeField] private UnitController currentUnit;
    [SerializeField] private List<UnitController> units;

    [SerializeField] private Dialogue dialogue;
    [SerializeField] private float relationship;

    public bool IsActive => isActive;
    public UnitController Unit => currentUnit;
    public Dialogue Dialogue => dialogue;
    public float Relationship => relationship;

    public event UnityAction OnIsActiveChanged;
    public event UnityAction OnInteractionStart;
    public event UnityAction OnDialogueStart;
    public event UnityAction<Response> OnDialogueResponse;
    public event UnityAction OnGiveComplete;
    public event UnityAction OnDialogueComplete;

    // used for when interating with a fungal by itself
    // opens basic menu with action buttons
    public void StartIntroDialogue(UnitController unit)
    {
        unit.Dialogue.StartDialogue(playerReference.Player);
        ApplyDialogue(unit, unit.Dialogue.RandomDialogue);
        OnInteractionStart?.Invoke();
    }

    private UnityAction onComplete;
    // used for when interacting with a funal by itself
    // immedialey opens dialogue, with no action buttons
    public void StartImmediateChat(UnitController unit, Dialogue dialogue, UnityAction onComplete = null)
    {
        unit.Dialogue.StartDialogue(playerReference.Player);
        ApplyDialogue(unit, dialogue);
        OnDialogueStart?.Invoke();
        this.onComplete = onComplete;
    }

    // used for when two fungals are already talking
    // the player joining in, creates a group dialogue
    public void StartGroupDialogue(Vector3 origin, GreetingDialogue dialogue, List<UnitController> units)
    {
        this.units = units;
        UnitController.ArrangeUnitsInRadius(origin, units);
        ApplyDialogue(dialogue.UnitA, dialogue.Dialogue);
        OnDialogueStart?.Invoke();
    }

    private void ApplyDialogue(UnitController unit, Dialogue dialogue)
    {
        //Debug.Log("setting target");
        currentUnit = unit;
        this.dialogue = dialogue;

        isActive = true;
        OnIsActiveChanged?.Invoke();

        unit.Focus();
        if (navigation.CurrentView != dialogueView) navigation.Navigate(dialogueView);
    }

    public void RespondToChat(Response response)
    {
        Debug.Log($"RespondToChat {response.Next}");

        if (response.Next != null) dialogue = response.Next;
        relationship += response.Relationship;
        OnDialogueResponse?.Invoke(response);
    }

    public void ContinueDialogue()
    {
        Debug.Log($"ContinueDialogue {dialogue.Next.Action}");

        if (dialogue.Next != null)
        {
            if (currentUnit != dialogue.Next.Unit)
            {
                currentUnit.Unfocus();
                currentUnit = dialogue.Next.Unit;
                currentUnit.Focus();
            }

            dialogue = dialogue.Next;
        }

        relationship += 50f;
    }

    public void CompleteDialogue()
    {
        //Debug.Log($"CompleteDialogue");

        currentUnit.Unfocus();
        OnDialogueComplete?.Invoke();

        foreach(var unit in units)
        {
            unit.Dialogue.CompleteDialogue();
        }

        navigation.GoBackToRoot();

        currentUnit = null;
        units = new List<UnitController>();
        dialogue = null;
        isActive = false;
        OnIsActiveChanged?.Invoke();

        onComplete?.Invoke();
        onComplete = null;
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
        dialogue = currentUnit.Dialogue.Dialogue[0];
        OnGiveComplete?.Invoke();
        navigation.GoBack();
    }

    public void StartFollow()
    {
        if (Unit is FungalController fungal)
        {
            fungal.Follow(playerReference.Player.GetComponent<UnitFollow>());
            CompleteDialogue();
        }
    }
}
