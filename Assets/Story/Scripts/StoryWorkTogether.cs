using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryWorkTogether : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StoryReference storyReference;
    [SerializeField] private StoryData prerequisite;
    [SerializeField] private StoryData workTogether;
    [SerializeField] private SceneNavigation sceneNavigation;
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private PlayerReference playerReference;
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private DialogueReference dialogueReference;
    [SerializeField] private List<Dialogue> workTogetherDialogue;
    [SerializeField] private InitialUI initialUI;
    [SerializeField] private Transform playerAnchor;
    [SerializeField] private Transform frogAnchor;
    [SerializeField] private GameObject plantGift;

    private void OnEnable()
    {
        sceneNavigation.OnSceneFadeIn += SceneNavigation_OnSceneFadeIn;
        partyReference.OnPartyDebriefComplete += PartyReference_OnPartyDebriefComplete;
    }

    private void OnDisable()
    {
        partyReference.OnPartyDebriefComplete -= PartyReference_OnPartyDebriefComplete;
        partyReference.OnPartyDebriefComplete -= PartyReference_OnPartyDebriefComplete;
    }

    private void SceneNavigation_OnSceneFadeIn()
    {
        if (!storyReference.HasCompleted(workTogether))
        {
            //initialUI.enabled = false;
            PartyReference_OnPartyDebriefComplete();
        }
    }

    private void PartyReference_OnPartyDebriefComplete()
    {
        if (storyReference.CompletedStories.Contains(workTogether)) return;
        if (!storyReference.CompletedStories.Contains(prerequisite)) return;

        StartCoroutine(StoryRoutine());
    }

    private IEnumerator StoryRoutine()
    {
        yield return new WaitForSeconds(1f);

        var partyFrog = unitManager.UnitControllers[0];
        partyFrog.SetBehaviour(partyFrog.GetComponent<UnitDJ>());
        partyFrog.transform.position = frogAnchor.position;
        partyFrog.SetLookPosition(playerAnchor.position);
        (partyFrog as FungalController).Focus();

        playerReference.Player.transform.position = playerAnchor.position;
        playerReference.Player.SetLookPosition(frogAnchor.position);

        cameraPanController.CenterTargetInView(partyFrog.transform.position);
        dialogueReference.StartDialogue(partyFrog, workTogetherDialogue);

        yield return new WaitUntil(() => dialogueReference.CurrentDialogue.Action == DialogueAction.GIFT);
        plantGift.SetActive(true);

        yield return new WaitWhile(() => dialogueReference.IsActive);
        plantGift.SetActive(false);

        (partyFrog as FungalController).Unfocus();
    }
}
