using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class ViewController : MonoBehaviour
{
    [SerializeField] private GameObject currentUI;
    [SerializeField] private GameObject targetUI;
    [SerializeField] private Button exitButton;

    private OverheadInteractionIndicator overheadInteraction;
    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        overheadInteraction = GetComponentInChildren<OverheadInteractionIndicator>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        var proximityAction = GetComponentInChildren<ProximityAction>();
        proximityAction.OnUse += () => ToggleView(true);

        exitButton.onClick.AddListener(() => ToggleView(false));

        ToggleView(false);

    }

    private void ToggleView(bool value)
    {
        currentUI.SetActive(!value);
        targetUI.SetActive(value);

        overheadInteraction.gameObject.SetActive(!value);
        virtualCamera.Priority = value ? 2 : 0;
    }
}
