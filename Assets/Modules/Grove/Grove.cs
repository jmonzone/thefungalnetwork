using UnityEngine;

public class Grove : MonoBehaviour
{
    [SerializeField] private FungalController fungal;
    [SerializeField] private MoveCharacterJoystick joystick;

    private void Awake()
    {
        fungal.InitializePrefab(2);
    }

    private void Start()
    {
        var initializeController = GetComponent<InitializeController>();
        initializeController.Initialize(fungal);

        joystick.player = fungal.Movement;
    }
}
