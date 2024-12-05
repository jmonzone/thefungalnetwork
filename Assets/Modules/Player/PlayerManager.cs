using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Controllable playerController;
    [SerializeField] private Controller controller;

    private void Awake()
    {
        controller.SetController(playerController);
    }
}
