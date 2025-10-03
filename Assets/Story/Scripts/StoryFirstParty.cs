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

    [SerializeField] private ActivityReference danceActivity;
    [SerializeField] private Transform danceZoneAnchor;

    [SerializeField] private InitialUI initialUI;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference gameplayView;

    [SerializeField] private DialogueReference dialogueReference;
    [SerializeField] private List<Dialogue> lostDialogue;
    [SerializeField] private List<Dialogue> letsTakeAPhotoDialogue;
    [SerializeField] private List<Dialogue> afterPhotoTakenDialogue;

    private int timesInteractedWithGuests;

    private void OnEnable()
    {
        dialogueReference.OnInteractionStart += DialogueReference_OnDialogueStart;
    }

    private void OnDisable()
    {
        dialogueReference.OnInteractionStart -= DialogueReference_OnDialogueStart;
    }

    private void Awake()
    {
        if (!storyReference.HasCompleted(firstParty))
        {
            initialUI.enabled = false;
            unitManager.OnAllUnitsSummoned += UnitManager_OnAllUnitsSummoned;
        }
    }

    private void UnitManager_OnAllUnitsSummoned()
    {
        StartCoroutine(PartyRoutine());
    }

    private IEnumerator PartyRoutine()
    {
        yield return new WaitForFixedUpdate();
        partyReference.StartParty(tutorialParty);

        danceActivity.StartActivity(danceZoneAnchor.position, new List<UnitController>());
        danceActivity.AddUnit(partyReference.Guests[0]);

        //yield return new WaitUntil(() => timesInteractedWithGuests > 5);
        //yield return new WaitWhile(() => dialogueReference.IsActive);
        //yield return new WaitForSeconds(1f);

        //dialogueReference.StartDialogue(partyFrog, new Dialogue(letsTakeAPhotoDialogue, DialogueType.STORY));

        //yield return new WaitUntil(() => navigation.CurrentView == gameplayView);
        //yield return new WaitForSeconds(1f);

        //foreach(var guest in partyReference.Guests)
        //{
        //    guest.SetBehaviour(guest.GetComponent<UnitDJ>());
        //}

        //foreach (var unit in unitManager.AllUnits)
        //{
        //    var randomDirection = (Vector3)Random.insideUnitCircle.normalized;
        //    randomDirection.z = randomDirection.y;
        //    randomDirection.y = 0;

        //    unit.transform.position = guestPictureAnchor.transform.position + randomDirection * 1f;
        //}

        //playerReference.Player.transform.position = cameraPositionAnchor.position;

        //yield return new WaitForSeconds(1f);

        //photoReference.SetLookTarget(guestPictureAnchor.transform);
        //photoReference.StartPhotoView();

        //foreach (var unit in unitManager.AllUnits)
        //{
        //    unit.SetLookTarget(playerReference.Player.transform);
        //}


        //yield return new WaitWhile(() => photoReference.IsActive);
        //yield return new WaitForSeconds(1f);

        //dialogueReference.StartDialogue(partyFrog, new Dialogue(afterPhotoTakenDialogue, DialogueType.STORY));

        //yield return new WaitWhile(() => dialogueReference.IsActive);
        //yield return new WaitForSeconds(1f);

        //partyFrog.ApplyDefaultBehaviour();

        //yield return new WaitForSeconds(1f);

        //partyReference.StopParty();
        //storyReference.CompleteStory(firstParty);
    }

    private void DialogueReference_OnDialogueStart()
    {
        timesInteractedWithGuests++;
    }
}
