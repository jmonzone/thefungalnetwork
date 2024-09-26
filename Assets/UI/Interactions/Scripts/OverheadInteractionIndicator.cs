using UnityEngine;
using UnityEngine.UI;

public class OverheadInteractionIndicator : MonoBehaviour
{
    [SerializeField] private ProximityAction proximityAction;
    [SerializeField] private GameObject indicator;
    [SerializeField] private Button indicatorButton;

    private void Awake()
    {
        indicatorButton.onClick.AddListener(proximityAction.Use);
    }

    private void Update()
    {
        indicator.SetActive(proximityAction.Interactable);
    }
}
