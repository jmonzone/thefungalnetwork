using UnityEngine;
using UnityEngine.UI;

public class OverheadInteractionIndicator : MonoBehaviour
{
    [SerializeField] private GameObject indicator;
    [SerializeField] private Button indicatorButton;

    private Camera mainCamera;
    private ProximityAction proximityAction;

    private void Awake()
    {
        mainCamera = Camera.main;
        proximityAction = GetComponentInParent<ProximityAction>();
        indicatorButton.onClick.AddListener(proximityAction.Use);
    }

    private void Update()
    {
        indicator.SetActive(proximityAction.Interactable && proximityAction.InRange);

        var position = mainCamera.WorldToScreenPoint(proximityAction.transform.position);
        transform.position = position;
    }
}
