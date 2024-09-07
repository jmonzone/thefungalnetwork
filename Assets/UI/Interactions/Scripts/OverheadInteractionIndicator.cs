using UnityEngine;

public class OverheadInteractionIndicator : MonoBehaviour
{
    [SerializeField] private ProximityAction proximityAction;
    [SerializeField] private GameObject indicator;

    private void Update()
    {
        indicator.SetActive(proximityAction.Interactable);
    }
}
