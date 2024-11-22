using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(ProximityAction))]
public class ProximityActionView : MonoBehaviour
{
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference viewReference;

    private OverheadInteractionIndicator overheadInteraction;
    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        overheadInteraction = GetComponentInChildren<OverheadInteractionIndicator>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        var proximityAction = GetComponentInChildren<ProximityAction>();
        proximityAction.OnUse += () => viewReference.Open();

        navigation.OnNavigated += () => OnNavigated();
    }

    private void OnNavigated()
    {
        var usingView = navigation.History.Contains(viewReference);
        overheadInteraction.gameObject.SetActive(!usingView);
        virtualCamera.Priority = usingView ? 2 : 0;
    }
}
