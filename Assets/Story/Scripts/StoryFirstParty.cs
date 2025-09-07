using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryFirstParty : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PartyData tutorialParty;
    [SerializeField] private StoryData firstParty;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private StoryReference storyReference;
    [SerializeField] private PhotoReference photoReference;
    [SerializeField] private Transform guestPictureAnchor;
    [SerializeField] private Transform cameraPositionAnchor;
    [SerializeField] private InitialUI initialUI;
    [SerializeField] private ViewReference partyView;

    [SerializeField] private DialogueReference dialogueReference;
    [SerializeField] private List<Dialogue> lostDialogue;
    [SerializeField] private List<Dialogue> letsTakeAPhotoDialogue;
    [SerializeField] private List<Dialogue> afterPhotoTakenDialogue;

    private int timesInteractedWithGuests;

    private void OnEnable()
    {
        sceneNavigation.OnSceneFadeIn += StartParty;
        dialogueReference.OnDialogueStart += DialogueReference_OnDialogueStart;
    }

    private void OnDisable()
    {
        sceneNavigation.OnSceneFadeIn -= StartParty;
        dialogueReference.OnDialogueStart -= DialogueReference_OnDialogueStart;
    }

    private void Awake()
    {
        if (!storyReference.HasCompleted(firstParty))
        {
            //initialUI.enabled = false;
        }
    }

    private void StartParty()
    {
        if (!storyReference.HasCompleted(firstParty))
        {
            StartCoroutine(PartyRoutine());
            //initialUI.enabled = true;
        }
    }

    private IEnumerator PartyRoutine()
    {
        partyReference.StartParty(tutorialParty);
        var partyFrog = unitManager.UnitControllers[0];

        //cameraPanController.CenterTargetInView(playerReference.Player.transform.position);
        //dialogueReference.StartDialogue(playerReference.Player, lostDialogue);

        yield return new WaitUntil(() => timesInteractedWithGuests > 0);
        yield return new WaitWhile(() => dialogueReference.IsActive);
        yield return new WaitForSeconds(1f);

        dialogueReference.StartSpecialDialogue(partyFrog, letsTakeAPhotoDialogue);

        yield return new WaitWhile(() => dialogueReference.IsActive);
        yield return new WaitForSeconds(1f);

        foreach(var guest in partyReference.Guests)
        {
            guest.SetBehaviour(guest.GetComponent<UnitDJ>());
        }

        foreach (var unit in unitManager.AllUnits)
        {
            var randomDirection = (Vector3)Random.insideUnitCircle.normalized;
            randomDirection.z = randomDirection.y;
            randomDirection.y = 0;

            unit.transform.position = guestPictureAnchor.transform.position + randomDirection * 1f;
        }

        playerReference.Player.transform.position = cameraPositionAnchor.position;

        //cameraPanController.CenterTargetInView(guestPictureAnchor.transform.position);

        yield return new WaitForSeconds(1f);

        photoReference.SetLookTarget(guestPictureAnchor.transform);
        photoReference.StartPhotoView();

        foreach (var unit in unitManager.AllUnits)
        {
            unit.SetLookTarget(playerReference.Player.transform);
        }


        yield return new WaitWhile(() => photoReference.IsActive);
        yield return new WaitForSeconds(1f);

        dialogueReference.StartSpecialDialogue(partyFrog, afterPhotoTakenDialogue);

        yield return new WaitWhile(() => dialogueReference.IsActive);
        yield return new WaitForSeconds(1f);

        partyFrog.SetDefaultBehaviour();

        yield return new WaitForSeconds(1f);

        partyReference.StopParty();
        storyReference.CompleteStory(firstParty);
    }

    private void DialogueReference_OnDialogueStart()
    {
        timesInteractedWithGuests++;
    }
}
