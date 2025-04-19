using UnityEngine;

public class Grove : MonoBehaviour
{
    [SerializeField] private FungalController fungal;
    [SerializeField] private MoveCharacterJoystick joystick;

    private void Awake()
    {
        fungal.InitializePrefab(2);
        joystick.player = fungal.Movement;
    }

    private void Start()
    {
        var initializeController = GetComponent<InitializeController>();
        initializeController.Initialize(fungal);
    }    
}
