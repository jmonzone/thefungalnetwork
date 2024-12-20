using UnityEngine;
using UnityEngine.Events;

public class CrocodileInteraction : MonoBehaviour
{
    [SerializeField] private Controller controller;

    private Attackable attackable;
    private MovementController movement;
    private ProximityAction proximityAction;

    private bool isDefeated;
    private bool isMounted;

    public event UnityAction OnMounted;
    public event UnityAction OnUnmounted;

    private void Awake()
    {
        proximityAction = GetComponent<ProximityAction>();
        proximityAction.OnUse += Mount;

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

    private void Controller_OnUpdate()
    {
        if (controller.Movement != movement)
        {
            SyncUnmount();
            OnUnmounted?.Invoke();
        }

        UpdateIsInteractable();
    }

    private void UpdateIsInteractable()
    {
        //todo: add delay animation so that the crocodile is not interactable immediately
        proximityAction.SetInteractable(isDefeated && !isMounted);
    }

    private void Mount()
    {
        SyncMount();
        controller.SetMovement(movement);
        OnMounted?.Invoke();
    }

    public void SyncMount()
    {
        isMounted = true;
        attackable.Restore();
        GetComponentInChildren<Animator>().Play("Spin");
        UpdateIsInteractable();
    }

    public void SyncUnmount()
    {
        isMounted = false;
        UpdateIsInteractable();
    }
}
