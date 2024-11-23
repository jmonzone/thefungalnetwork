using Cinemachine;
using UnityEngine;

public class RabbitHole : MonoBehaviour
{
    private void Start()
    {
        var proximityAction = GetComponent<ProximityAction>();
        //proximityAction.OnUse += () => Utility.LoadScene("Pufferball");

        var virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        proximityAction.OnInRangeChanged += value =>
        {
            virtualCamera.Priority = value ? 1 : 0;
        };
    }
}
