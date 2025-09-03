using System.Collections;
using UnityEngine;

public class PartyTutorial : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private PhotoReference photoReference;

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
        StartCoroutine(PartyRoutine());
    }

    private IEnumerator PartyRoutine()
    {
        yield return new WaitForSeconds(2.5f);
        photoReference.SetLookTarget(partyReference.Guests[0].transform);
        photoReference.StartPhotoView();
    }
}
