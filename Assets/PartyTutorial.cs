using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyTutorial : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private PhotoReference photoReference;
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private Transform guestPictureAnchor;
    [SerializeField] private Transform cameraPositionAnchor;

    [SerializeField] private DialogueReference dialogueReference;
    [SerializeField] private List<Dialogue> initialDialogue;


    private int timesInteractedWithGuests;

    private void OnEnable()
    {
        partyReference.OnPartyStarted += PartyReference_OnPartyStarted;
        dialogueReference.OnDialogueStart += DialogueReference_OnDialogueStart;
    }

    private void OnDisable()
    {
        partyReference.OnPartyStarted -= PartyReference_OnPartyStarted;
        dialogueReference.OnDialogueStart -= DialogueReference_OnDialogueStart;
    }

    private void DialogueReference_OnDialogueStart()
    {
        timesInteractedWithGuests++;
    }

    private void Start()
    {
        partyReference.StartParty(partyReference.Parties[0]);
    }

    private void PartyReference_OnPartyStarted()
    {
        StartCoroutine(PartyRoutine());
    }

    private IEnumerator PartyRoutine()
    {
        yield return null;
        var initialUnit = unitManager.UnitControllers[0];
        initialUnit.SetBehaviour(initialUnit.GetComponent<UnitDJ>());

        yield return new WaitUntil(() => timesInteractedWithGuests > 0);
        yield return new WaitWhile(() => dialogueReference.IsActive);
        yield return new WaitForSeconds(1f);

        dialogueReference.StartDialogue(initialUnit, initialDialogue);
        cameraPanController.CenterTargetInView(initialUnit.transform.position);

        yield return new WaitWhile(() => dialogueReference.IsActive);
        yield return new WaitForSeconds(1f);

        foreach(var guest in partyReference.Guests)
        {
            var randomDirection = (Vector3)Random.insideUnitCircle.normalized;
            randomDirection.z = randomDirection.y;
            randomDirection.y = 0;

            guest.transform.position = guestPictureAnchor.transform.position + randomDirection * 0.5f;
            guest.SetBehaviour(guest.GetComponent<UnitDJ>());
        }

        playerReference.Player.transform.position = cameraPositionAnchor.position;

        cameraPanController.CenterTargetInView(guestPictureAnchor.transform.position);

        yield return new WaitForSeconds(1f);

        photoReference.SetLookTarget(guestPictureAnchor.transform);
        photoReference.StartPhotoView();

        while (true)
        {
            foreach (var guest in partyReference.Guests)
            {
                guest.SetLookPosition(playerReference.Player.transform.position);
            }

            yield return null;
        }
    }
}
