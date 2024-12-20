using UnityEngine;

public class CrocodileInteraction : MonoBehaviour
{
    [SerializeField] private Controller controller;

    private Attackable attackable;
    private MovementController movement;
    private ProximityAction proximityAction;

    private void Awake()
    {
        proximityAction = GetComponent<ProximityAction>();
        proximityAction.OnUse += ProximityAction_OnUse;

        attackable = GetComponent<Attackable>();
        attackable.OnHealthChanged += Attackable_OnHealthChanged;
        Attackable_OnHealthChanged();

        movement = GetComponent<MovementController>();
    }

    private void Attackable_OnHealthChanged()
    {
        //todo: add delay animation so that the crocodile is not interactable immediately
        proximityAction.SetInteractable(attackable.CurrentHealth == 0);
    }

    private void ProximityAction_OnUse()
    {
        controller.SetMovement(movement);
    }

}
