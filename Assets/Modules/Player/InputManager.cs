using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Button interactionButton;
    [SerializeField] private Controller controller;
    [SerializeField] private Volume volume;

    public IControllable Controllable { get; private set; }

    public event UnityAction<MovementController> OnMovementChanged;

    private void Awake()
    {
        interactionButton.onClick.AddListener(() =>
        {
            controller.Interactions.TargetAction.Use();
        });


        virtualJoystick.OnJoystickEnd += () =>
        {
            if (Controllable == null) return;
            Controllable.Movement.Stop();
        };

        virtualJoystick.OnJoystickUpdate += input =>
        {
            if (Controllable == null) return;
            var direction = new Vector3(input.x, 0, input.y);
            direction = Quaternion.Euler(0, cameraController.transform.GetChild(0).eulerAngles.y, 0) * direction;
            Controllable.Movement.SetDirection(direction);
        };

        controller.Volume = volume;
    }

    public void SetControllable(IControllable controller, ProximityInteraction interaction)
    {
        this.controller.SetController(controller.Movement, interaction);
        Controllable = controller;
        Controllable.Movement.Stop();
        cameraController.Target = Controllable.Movement.transform;
        OnMovementChanged?.Invoke(Controllable.Movement);
    }

    public void CanInteract(bool value)
    {
        interactionButton.interactable = value;
    }


    private ProximityAction previousAction;

    private void Update()
    {
        var targetAction = controller.Interactions.TargetAction;

        if (previousAction && previousAction != targetAction) previousAction.SetInRange(false);
        previousAction = targetAction;

        if (targetAction) targetAction.SetInRange(true);
        CanInteract(targetAction && targetAction.Interactable);
    }
}
