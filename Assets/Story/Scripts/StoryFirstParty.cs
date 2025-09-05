using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private Transform guestPictureAnchor;
    [SerializeField] private Transform cameraPositionAnchor;
    [SerializeField] private Transform frogSpawnAnchor;

    [SerializeField] private DialogueReference dialogueReference;
    [SerializeField] private List<Dialogue> lostDialogue;
    [SerializeField] private List<Dialogue> letsTakeAPhotoDialogue;
    [SerializeField] private List<Dialogue> afterPhotoTakenDialogue;

    private int timesInteractedWithGuests;

    private void OnEnable()
    {
        sceneNavigation.OnSceneFadeIn += SceneNavigation_OnSceneFadeIn;
        dialogueReference.OnDialogueStart += DialogueReference_OnDialogueStart;
    }

    private void SceneNavigation_OnSceneFadeIn()
    {
        if (!storyReference.HasCompleted(firstParty))
        {
            StartCoroutine(PartyRoutine());

            partyReference.StartParty(tutorialParty);
        }
    }

    private void OnDisable()
    {
        sceneNavigation.OnSceneFadeIn -= SceneNavigation_OnSceneFadeIn;
        dialogueReference.OnDialogueStart -= DialogueReference_OnDialogueStart;
    }

    private void DialogueReference_OnDialogueStart()
    {
        timesInteractedWithGuests++;
    }

    private IEnumerator PartyRoutine()
    {
        yield return new WaitForSeconds(1f);

        var partyFrog = unitManager.UnitControllers[0];
        partyFrog.SetBehaviour(partyFrog.GetComponent<UnitDJ>());
        partyFrog.transform.position = frogSpawnAnchor.position;


        cameraPanController.CenterTargetInView(playerReference.Player.transform.position + Vector3.back * 20f);
        dialogueReference.StartDialogue(playerReference.Player, lostDialogue);

        yield return new WaitUntil(() => timesInteractedWithGuests > 1);
        yield return new WaitWhile(() => dialogueReference.IsActive);
        yield return new WaitForSeconds(1f);

        cameraPanController.CenterTargetInView(partyFrog.transform.position);
        dialogueReference.StartDialogue(partyFrog, letsTakeAPhotoDialogue);

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

            unit.transform.position = guestPictureAnchor.transform.position + randomDirection * 0.25f;
        }

        playerReference.Player.transform.position = cameraPositionAnchor.position;

        cameraPanController.CenterTargetInView(guestPictureAnchor.transform.position);

        yield return new WaitForSeconds(1f);

        photoReference.SetLookTarget(guestPictureAnchor.transform);
        photoReference.StartPhotoView();

        while (photoReference.IsActive)
        {
            foreach (var unit in unitManager.AllUnits)
            {
                unit.SetLookPosition(playerReference.Player.transform.position);
            }

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        dialogueReference.StartDialogue(partyFrog, afterPhotoTakenDialogue);
        cameraPanController.CenterTargetInView(partyFrog.transform.position);

        yield return new WaitWhile(() => dialogueReference.IsActive);
        yield return new WaitForSeconds(1f);

        partyFrog.SetDefaultBehaviour();

        partyReference.ClearGuests();

        yield return new WaitForSeconds(1f);

        partyReference.StopParty();
        storyReference.CompleteStory(firstParty);
    }
}
