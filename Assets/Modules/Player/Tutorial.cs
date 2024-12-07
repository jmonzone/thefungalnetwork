using UnityEngine;

// todo: see if initalizing controller logic can be shared with GroveManager
public class Tutorial : MonoBehaviour
{
    [SerializeField] private Controllable avatar;
    [SerializeField] private Controller controller;
    [SerializeField] private ViewReference inputView;

    private void Start()
    {
        controller.SetController(avatar);
        inputView.Open();
    }
}
