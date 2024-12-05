using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Button interactionButton;
    [SerializeField] private Controller controller;
    [SerializeField] private Volume volume;

    private void Awake()
    {
        interactionButton.onClick.AddListener(() =>
        {
            controller.Interactions.TargetAction.Use();
        });


        virtualJoystick.OnJoystickEnd += () =>
        {
            if (controller == null) return;
            controller.Movement.Stop();
        };

        virtualJoystick.OnJoystickUpdate += input =>
        {
            if (controller == null) return;
            var direction = new Vector3(input.x, 0, input.y);
            direction = Quaternion.Euler(0, cameraController.transform.GetChild(0).eulerAngles.y, 0) * direction;
            controller.Movement.SetDirection(direction);
        };

        controller.Volume = volume;
        controller.OnUpdate += () =>
        {
            cameraController.Target = controller.Movement.transform;
        };
    }

    public void CanInteract(bool value)
    {
        interactionButton.interactable = value;
    }


    private ProximityAction previousAction;

    private void Update()
    {
        if (!controller.Interactions) return;

        var targetAction = controller.Interactions.TargetAction;

        if (previousAction && previousAction != targetAction) previousAction.SetInRange(false);
        previousAction = targetAction;

        if (targetAction) targetAction.SetInRange(true);
        CanInteract(targetAction && targetAction.Interactable);
    }
}
