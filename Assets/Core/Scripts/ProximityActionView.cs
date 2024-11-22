using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(ProximityAction))]
public class ProximityActionView : MonoBehaviour
{
    [SerializeField] private ViewReference viewReference;

    private OverheadInteractionIndicator overheadInteraction;
    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        overheadInteraction = GetComponentInChildren<OverheadInteractionIndicator>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        var proximityAction = GetComponentInChildren<ProximityAction>();
        proximityAction.OnUse += () => viewReference.Open();

        viewReference.OnOpened += () => OnViewToggled(true);
        viewReference.OnClosed += () => OnViewToggled(false);
    }

    private void OnViewToggled(bool value)
    {
        overheadInteraction.gameObject.SetActive(!value);
        virtualCamera.Priority = value ? 2 : 0;
    }
}
