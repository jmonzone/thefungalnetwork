using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InputManager inputManager;

    private ProximityAction previousAction;

    private void Awake()
    {
        inputManager.SetControllable(playerController);

        inputManager.OnInteractionButtonClicked += () =>
        {
            playerController.Interactions.TargetAction.Use();
        };
    }

    private void Update()
    {
        var targetAction = playerController.Interactions.TargetAction;

        if (previousAction && previousAction != targetAction) previousAction.SetInRange(false);
        previousAction = targetAction;

        if (targetAction) targetAction.SetInRange(true);
        inputManager.CanInteract(targetAction && targetAction.Interactable);
    }
}
