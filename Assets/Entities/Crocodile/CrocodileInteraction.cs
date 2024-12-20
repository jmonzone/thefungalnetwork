using UnityEngine;
using UnityEngine.Events;

public class CrocodileInteraction : MonoBehaviour
{
    [SerializeField] private Controller controller;

    private Attackable attackable;
    private MovementController movement;
    private ProximityAction proximityAction;
    private CrocodileAI crocodileAI;
    private bool isRestored;
    private bool isMounted;

    public event UnityAction OnMounted;

    private void Awake()
    {
        proximityAction = GetComponent<ProximityAction>();
        proximityAction.OnUse += Mount;

        attackable = GetComponent<Attackable>();
        attackable.OnHealthChanged += Attackable_OnHealthChanged; ;
        UpdateIsInteractable();

        movement = GetComponent<MovementController>();

        crocodileAI = GetComponent<CrocodileAI>();
    }

    private void Attackable_OnHealthChanged()
    {
        if (attackable.CurrentHealth == 0)
        {
            isRestored = true;
            UpdateIsInteractable();
        }
    }

    private void OnEnable()
    {
        controller.OnUpdate += Controller_OnUpdate;
    }

    private void Controller_OnUpdate()
    {
        isMounted = controller.Movement == movement;
        UpdateIsInteractable();
    }

    private void UpdateIsInteractable()
    {
        //todo: add delay animation so that the crocodile is not interactable immediately
        proximityAction.SetInteractable(isRestored && !isMounted);
    }

    private void Mount()
    {
        crocodileAI.enabled = false;
        Restore();
        controller.SetMovement(movement);
        OnMounted?.Invoke();
    }

    public void Restore()
    {
        attackable.Restore();
        GetComponentInChildren<Animator>().Play("Spin");
        UpdateIsInteractable();
    }

}
