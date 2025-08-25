using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyGuestSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnAnchor;
    [SerializeField] private UnitController unitPrefab;
    [SerializeField] private UnitListReference unitListReference;

    [SerializeField] private List<UnitController> guests;

    public List<UnitController> Guests => guests;

    private void Awake()
    {
        var partyManager = GetComponent<PartyHUDUI>();
        partyManager.OnGuestArrived += PartyReference_OnGuestArrived;
    }

    private void PartyReference_OnGuestArrived(Unit guestUnit)
    {
        var guest = Instantiate(unitPrefab, spawnAnchor.transform.position, Quaternion.identity);
        guest.Initialize(guestUnit);

        guests.Add(guest);
    }
}
