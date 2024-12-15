using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private ProximityAction proximityAction;
    [SerializeField] private Tutorial tutorial;

    private void Awake()
    {
        proximityAction.OnUse += () => tutorial.SetIsCompletd(true);
    }
}
