using System.Collections;
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
    private ValueBarParticleController valueBarParticleController;

    private void Awake()
    {
        //valueBarController = GetComponent<ValueBarController>();
        //valueBarParticleController = GetComponent<ValueBarParticleController>();

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

        //partyReference.OnVibeIncreased += PartyReference_OnVibeIncreased;

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

        //partyReference.OnVibeIncreased -= PartyReference_OnVibeIncreased;
    }

    private void PartyReference_OnPartyStarted()
    {
        var partyFrog = unitManager.UnitControllers[0];
        partyFrog.SetBehaviour(partyFrog.GetComponent<UnitDJ>());

        foreach(var guest in partyReference.CurrentParty.Guests)
        {
            // Try to find a valid random position near the spawn anchor
            Vector3 randomPoint = Random.insideUnitSphere * 2f; // radius = 5 units
            randomPoint.y = spawnAnchor.transform.position.y; // keep roughly at same height

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                // Use closest valid point on NavMesh
                randomPoint = hit.position;
            }
            else
            {
                // Fallback: spawn at anchor
                randomPoint = spawnAnchor.transform.position;
            }

            // Spawn guest
            var instance = unitList.RegisterUnit(guest, 0);

            var controller = Instantiate(unitPrefab, randomPoint, Quaternion.identity);
            controller.Initialize(instance);
            partyReference.AddGuest(controller);
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
        valueBarParticleController.BurstFromWorld(points, position);
    }


}
