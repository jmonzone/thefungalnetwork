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
    [SerializeField] private DialogueReference dialogueReference;
    [SerializeField] private List<Dialogue> workTogetherDialogue;
    [SerializeField] private List<Dialogue> buildCompleteDialogue;
    [SerializeField] private Transform playerAnchor;
    [SerializeField] private Transform frogAnchor;
    [SerializeField] private GameObject plantGift;
    [SerializeField] private BuildReference buildReference;
    [SerializeField] private Item plantItem;

    private void OnEnable()
    {
        sceneNavigation.OnSceneFadeIn += TryStartStory;
        partyReference.OnPartyDebriefComplete += TryStartStory;
    }

    private void OnDisable()
    {
        sceneNavigation.OnSceneFadeIn += TryStartStory;
        partyReference.OnPartyDebriefComplete -= TryStartStory;
    }

    private void TryStartStory()
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
        playerReference.Player.transform.position = playerAnchor.position;
        playerReference.Player.SetLookPosition(frogAnchor.position);

        dialogueReference.StartDialogue(partyFrog, new Dialogue(workTogetherDialogue, DialogueType.STORY));

        yield return new WaitUntil(() => dialogueReference.Dialogue.Action == DialogueAction.GIFT);

        plantGift.SetActive(true);

        yield return new WaitWhile(() => dialogueReference.IsActive);
        plantGift.SetActive(false);

        buildReference.StartBuildPlacement(plantItem);

        yield return new WaitWhile(() => buildReference.CurrentBuild);

        buildReference.EndBuild();

        dialogueReference.StartDialogue(partyFrog, new Dialogue(buildCompleteDialogue, DialogueType.STORY));
        yield return new WaitWhile(() => dialogueReference.IsActive);

        storyReference.CompleteStory(workTogether);
        partyReference.ShowPartyList();
    }
}
