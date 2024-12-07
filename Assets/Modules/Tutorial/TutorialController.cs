using UnityEngine;

// todo: see if initalizing controller logic can be shared with GroveManager
// todo: try to combine scene start logic between laucnher, tutorial, and grove
public class TutorialController : MonoBehaviour
{
    [SerializeField] private Controllable avatar;
    [SerializeField] private Controller controller;
    [SerializeField] private ViewReference inputView;
    [SerializeField] private SceneNavigation sceneNavigation;

    [SerializeField] private ProximityAction proximityAction;
    [SerializeField] private Tutorial tutorial;

    private void Awake()
    {
        proximityAction.OnUse += () => tutorial.SetIsCompletd(true);

        sceneNavigation.OnSceneLoaded += () =>
        {
            inputView.RequestShow();
        };
    }

    private void Start()
    {
        controller.SetController(avatar);
    }
}
