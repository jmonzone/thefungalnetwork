using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyGuestSpawner : MonoBehaviour
{
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private Transform spawnAnchor;
    [SerializeField] private UnitController unitPrefab;
    [SerializeField] private UnitListReference unitListReference;



    private void Awake()
    {
        var partyManager = GetComponent<PartyHUDUI>();
        partyManager.OnPhaseChanged += PartyManager_OnPhaseChanged;
    }

    private void PartyManager_OnPhaseChanged(PartyPhase phase)
    {
        switch (phase)
        {
            case PartyPhase.DOORS_OPEN:
                StartCoroutine(DoorsOpenRoutine());
                break;
            case PartyPhase.WIND_DOWN:
                partyReference.ClearGuests();
                break;
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
