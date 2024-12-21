using UnityEngine;
using UnityEngine.Events;

//todo: centralize with mushroomobjective to create a central moutn component
public class CrocodileInteraction : MonoBehaviour
{
    [SerializeField] private Controller controller;
    //[SerializeField] private GameObject mountIndicator;

    private Attackable attackable;
    private MovementController movement;
    private ProximityAction proximityAction;

    private bool isDefeated;
    private bool isMounted;

    public event UnityAction OnMountRequested;
    public event UnityAction OnUnmounted;

    private void Awake()
    {
        proximityAction = GetComponent<ProximityAction>();
        proximityAction.OnUse += RequestMount;

        attackable = GetComponent<Attackable>();
        attackable.OnHealthChanged += Attackable_OnHealthChanged; ;
        UpdateIsInteractable();

        movement = GetComponent<MovementController>();
    }

    private void Attackable_OnHealthChanged()
    {
        if (attackable.CurrentHealth == 0)
        {
            isDefeated = true;
            UpdateIsInteractable();
        }
    }

    private void OnEnable()
    {
        controller.OnUpdate += Controller_OnUpdate;
    }

    private void OnDisable()
    {
        controller.OnUpdate -= Controller_OnUpdate;
    }


    private void Controller_OnUpdate()
    {
        if (isMounted && controller.Movement != movement)
        {
            RequestUnmount();
        }

        UpdateIsInteractable();
    }

    private void UpdateIsInteractable()
    {
        //todo: add delay animation so that the crocodile is not interactable immediately
        proximityAction.SetInteractable(isDefeated && !isMounted);
    }

    private void RequestMount()
    {
        HandleMount(true);
        controller.SetMovement(movement);
        OnMountRequested?.Invoke();
    }

    public void HandleMount(bool value)
    {
        isMounted = value;
        //mountIndicator.SetActive(value);

        if (value)
        {
            attackable.Restore();
            GetComponentInChildren<Animator>().Play("Spin");
        }
        
        UpdateIsInteractable();
    }

    public void RequestUnmount()
    {
        HandleMount(false);
        OnUnmounted?.Invoke();
    }
}
