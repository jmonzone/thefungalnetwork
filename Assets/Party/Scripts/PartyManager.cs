using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private UnitListReference unitList;
    [SerializeField] private Transform frogAnchor;
    [SerializeField] private UnitController unitPrefab;
    [SerializeField] private Transform spawnAnchor;
    [SerializeField] private float currentTimer;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference gameplayView;
    [SerializeField] private FadeCanvasGroup vibeMeterCanvas;

    private ValueBarController valueBarController;
    private ValueBarParticleManager valueBarParticleController;

    private void Awake()
    {
        valueBarController = GetComponent<ValueBarController>();
        valueBarParticleController = GetComponent<ValueBarParticleManager>();

        //valueBarParticleController.OnParticleReached += ValueBarParticleController_OnParticlesReached;
    }

    private void ValueBarParticleController_OnParticlesReached()
    {
        valueBarController.Increment();
    }

    private void OnEnable()
    {
        partyReference.OnPartyStarted += PartyReference_OnPartyStarted;
        partyReference.OnPartyComplete += PartyReference_OnPartyComplete;

        partyReference.OnVibeIncreased += PartyReference_OnVibeIncreased;

        navigation.OnNavigated += Navigation_OnNavigated;
    }

    private void Navigation_OnNavigated()
    {
        if (partyReference.IsActive && navigation.CurrentView == gameplayView)
        {
            StartCoroutine(vibeMeterCanvas.FadeIn());
        }
        else
        {
            StartCoroutine(vibeMeterCanvas.FadeOut());
        }
    }

    private void OnDisable()
    {
        partyReference.OnPartyStarted -= PartyReference_OnPartyStarted;
        partyReference.OnPartyComplete -= PartyReference_OnPartyComplete;

        partyReference.OnVibeIncreased -= PartyReference_OnVibeIncreased;
    }

    private void PartyReference_OnPartyStarted()
    {
        //todo: apply jobs to all units - party frog as dj is default
        var partyFrog = unitManager.UnitControllers[0];
        partyFrog.SetDefaultBehaviour(partyFrog.GetComponent<UnitDJ>());

        return;

        //todo: assemble guests predefined by party + newly introduced friends
        var allGuests = new List<UnitInstance>();

        //todo: don't spawn host or guests already spawned;
        //foreach(var predefinedGuest in partyReference.CurrentParty.Guests)
        //{
        //    var copyGuest = unitList.CopyUnit(predefinedGuest, false);
        //    allGuests.Add(copyGuest);
        //}

        //// todo: include direct invites
        //// todo: introduces guests
        //foreach (var guest in unitList.Friends)
        //{
        //    var friendshipLevel = guest.FriendshipLevel;

        //    if (friendshipLevel >= 2 && unitList.TryGetFriend(guest, out UnitInstance friend, allGuests))
        //    {
        //        allGuests.Add(friend);
        //    }
        //}

        foreach (var guest in allGuests)
        {
            var unit = unitManager.SummonUnit(guest);
            partyReference.AddGuest(unit);
        }

        StartCoroutine(vibeMeterCanvas.FadeIn());
        StartCoroutine(PartyRoutine());
    }

    private IEnumerator PartyRoutine()
    {
        if (partyReference.CurrentParty.Duration > 0)
        {
            currentTimer = 0;

            while (currentTimer < partyReference.CurrentParty.Duration)
            {
                if (Input.GetKeyUp(KeyCode.E)) break;
                currentTimer += Time.deltaTime;
                yield return null;
            }

            partyReference.StopParty();
        }
    }

    private void PartyReference_OnPartyComplete()
    {
        StartCoroutine(EndParty());
    }

    private IEnumerator EndParty()
    {
        yield return vibeMeterCanvas.FadeOut();
        StopAllCoroutines();
    }


    private void PartyReference_OnVibeIncreased(int points, Vector3 position)
    {
        //valueBarParticleController.BurstFromWorld(points, position);
    }


}
