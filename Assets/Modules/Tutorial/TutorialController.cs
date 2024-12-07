using UnityEngine;

// todo: see if initalizing controller logic can be shared with GroveManager
public class TutorialController : MonoBehaviour
{
    [SerializeField] private Controllable avatar;
    [SerializeField] private Controller controller;

    [SerializeField] private ProximityAction proximityAction;
    [SerializeField] private Tutorial tutorial;

    private void Awake()
    {
        proximityAction.OnUse += () => tutorial.SetIsCompletd(true);
    }

    private void Start()
    {
        controller.SetController(avatar);
    }
}
