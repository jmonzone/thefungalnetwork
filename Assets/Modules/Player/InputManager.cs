using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Button interactionButton;
    [SerializeField] private Controller controller;

    public IControllable Controllable { get; private set; }

    public event UnityAction<MovementController> OnMovementChanged;
    public event UnityAction OnInteractionButtonClicked;

    private void Awake()
    {
        interactionButton.onClick.AddListener(() => OnInteractionButtonClicked?.Invoke());

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
    }

    public void SetControllable(IControllable controller)
    {
        this.controller.SetMovement(controller.Movement);
        Controllable = controller;
        Controllable.Movement.Stop();
        cameraController.Target = Controllable.Movement.transform;
        OnMovementChanged?.Invoke(Controllable.Movement);
    }

    public void CanInteract(bool value)
    {
        interactionButton.interactable = value;
    }
}
