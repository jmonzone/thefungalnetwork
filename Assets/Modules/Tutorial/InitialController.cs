using UnityEngine;

public class InitialController : MonoBehaviour
{
    [SerializeField] private Controllable avatar;
    [SerializeField] private Controller controller;

    private void Start()
    {
        controller.SetController(avatar);
    }
}
