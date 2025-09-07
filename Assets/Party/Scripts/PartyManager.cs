using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private Transform frogAnchor;
    [SerializeField] private UnitController unitPrefab;
    [SerializeField] private Transform spawnAnchor;
    [SerializeField] private float currentTimer;
    [SerializeField] private FadeCanvasGroup vibeMeterCanvas;

    private void OnEnable()
    {
        partyReference.OnPartyStarted += PartyReference_OnPartyStarted;
        partyReference.OnPartyComplete += PartyReference_OnPartyComplete;
    }

    private void OnDisable()
    {
        partyReference.OnPartyStarted -= PartyReference_OnPartyStarted;
        partyReference.OnPartyComplete -= PartyReference_OnPartyComplete;
    }

    private void PartyReference_OnPartyStarted()
    {
        var partyFrog = unitManager.UnitControllers[0];
        partyFrog.SetBehaviour(partyFrog.GetComponent<UnitDJ>());

        currentTimer = 0;

        StartCoroutine(PartyRoutine());
    }

    private IEnumerator PartyRoutine()
    {
        yield return vibeMeterCanvas.FadeIn();

        int guestsToSpawn = partyReference.CurrentParty.Guests.Count;

        for (int i = 0; i < guestsToSpawn; i++)
        {
            yield return null;

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
            var guest = Instantiate(unitPrefab, randomPoint, Quaternion.identity);
            guest.Initialize(partyReference.CurrentParty.Guests[i]);
            partyReference.AddGuest(guest);
        }

        if (partyReference.CurrentParty.Duration > 0)
        {
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
        StartCoroutine(vibeMeterCanvas.FadeOut());
        StopAllCoroutines();
    }
}
