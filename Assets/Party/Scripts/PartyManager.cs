using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Navigation navigation;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private Transform frogAnchor;
    [SerializeField] private UnitController unitPrefab;

    [SerializeField] private Transform spawnAnchor;

    [SerializeField] private float currentTimer;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            navigation.GoBack(2);
        });
    }

    private void OnEnable()
    {
        partyReference.OnPartyStarted += PartyReference_OnPartyStarted;
    }

    private void OnDisable()
    {
        partyReference.OnPartyStarted -= PartyReference_OnPartyStarted;
    }

    private void PartyReference_OnPartyStarted()
    {
        var partyFrog = unitManager.UnitControllers[0];
        partyFrog.SetBehaviour(partyFrog.GetComponent<UnitDJ>());
        partyFrog.transform.position = frogAnchor.position;

        currentTimer = 0;
        slider.minValue = 0;
        slider.maxValue = partyReference.CurrentParty.Duration;

        StartCoroutine(PartyRoutine());
    }

    private IEnumerator PartyRoutine()
    {
        Debug.Log($"{currentTimer} {partyReference.CurrentParty.Duration}");
        int guestsToSpawn = partyReference.CurrentParty.Guests.Count;

        for (int i = 0; i < guestsToSpawn; i++)
        {
            yield return null;

            // Try to find a valid random position near the spawn anchor
            Vector3 randomPoint = Random.insideUnitSphere * 2f; // radius = 5 units
            randomPoint.y = spawnAnchor.transform.position.y; // keep roughly at same height
            Quaternion randomYRotation = Quaternion.Euler(0, Random.Range(135f, 225f), 0);

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

        while (currentTimer < partyReference.CurrentParty.Duration)
        {
            currentTimer += Time.deltaTime;
            slider.value = currentTimer;

            yield return null;
        }

        partyReference.StopParty();
    }
}
