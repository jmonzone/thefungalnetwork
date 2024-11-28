using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InputManager inputManager;

    private void Awake()
    {
        inputManager.SetControllable(playerController);
    }
}
