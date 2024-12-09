using UnityEngine;
using UnityEngine.UI;

public class OverheadInteractionIndicator : MonoBehaviour
{
    [SerializeField] private GameObject indicator;
    [SerializeField] private Button indicatorButton;

    private ProximityAction proximityAction;

    private void Awake()
    {
        proximityAction = GetComponentInParent<ProximityAction>();
        indicatorButton.onClick.AddListener(proximityAction.Use);
    }

    private void Update()
    {
        indicator.SetActive(proximityAction.Interactable && proximityAction.InRange);
    }
}
