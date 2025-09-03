using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PartyGuestSpawner : MonoBehaviour
{
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private Transform spawnAnchor;
    [SerializeField] private UnitController unitPrefab;
    [SerializeField] private UnitListReference unitListReference;

    private void Awake()
    {
        var partyManager = GetComponent<PartyManager>();
        partyManager.OnPhaseChanged += PartyManager_OnPhaseChanged;
    }

    private void PartyManager_OnPhaseChanged(PartyPhase phase)
    {
        switch (phase)
        {
            case PartyPhase.DOORS_OPEN:
                StartCoroutine(SpawnImmediately());
                //StartCoroutine(DoorsOpenRoutine());
                break;
            case PartyPhase.WIND_DOWN:
                //partyReference.ClearGuests();
                break;
        }
    }

    private IEnumerator SpawnImmediately()
    {
        int guestsToSpawn = partyReference.CurrentParty.Guests.Count;

        for (int i = 0; i < guestsToSpawn; i++)
        {
            yield return null;

            // Try to find a valid random position near the spawn anchor
            Vector3 randomPoint = Random.insideUnitSphere * 2f; // radius = 5 units
            randomPoint.y = spawnAnchor.transform.position.y; // keep roughly at same height
            Quaternion randomYRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

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
            var guest = Instantiate(unitPrefab, randomPoint, randomYRotation);
            guest.Initialize(partyReference.CurrentParty.Guests[i]);
            partyReference.AddGuest(guest);
        }
    }


private IEnumerator DoorsOpenRoutine()
    {
        // Initial delay before the first guest arrives
        float initialDelay = Random.Range(0.5f, 2f);
        yield return new WaitForSeconds(initialDelay);

        int guestsToSpawn = partyReference.CurrentParty.Guests.Count;
        if (guestsToSpawn == 0)
            yield break;

        // Spread arrivals across the given phase duration
        float avgInterval = 7.5f / (guestsToSpawn + 1);

        for (int i = 0; i < guestsToSpawn; i++)
        {
            float randomizedInterval = avgInterval * Random.Range(0.8f, 1.2f);
            yield return new WaitForSeconds(randomizedInterval);

            var guest = Instantiate(unitPrefab, spawnAnchor.transform.position, Quaternion.identity);
            guest.Initialize(partyReference.CurrentParty.Guests[i]);
            partyReference.AddGuest(guest);
        }
    }


}
