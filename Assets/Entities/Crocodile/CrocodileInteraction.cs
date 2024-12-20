using UnityEngine;
using UnityEngine.Events;

public class CrocodileInteraction : MonoBehaviour
{
    [SerializeField] private Controller controller;

    private Attackable attackable;
    private MovementController movement;
    private ProximityAction proximityAction;
    private CrocodileAI crocodileAI;

    public event UnityAction OnMounted;

    private void Awake()
    {
        proximityAction = GetComponent<ProximityAction>();
        proximityAction.OnUse += Mount;

        attackable = GetComponent<Attackable>();
        attackable.OnHealthChanged += UpdateIsInteractable;
        UpdateIsInteractable();

        movement = GetComponent<MovementController>();

        crocodileAI = GetComponent<CrocodileAI>();
    }

    private void UpdateIsInteractable()
    {
        //todo: add delay animation so that the crocodile is not interactable immediately
        proximityAction.SetInteractable(attackable.CurrentHealth == 0);
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
