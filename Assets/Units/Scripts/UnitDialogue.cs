using UnityEngine;
using UnityEngine.Events;

public class UnitDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;

    [Header("Settings")]
    [SerializeField] private bool lookAtTarget = true;

    private UnitController unit;
    private Vector3 originalLookPosition;

    public event UnityAction OnDialogueStarted;
    public event UnityAction OnDialogueCompleted;

    private void Awake()
    {
        unit = GetComponent<UnitController>();
    }

    public void StartDialogue(UnitController target)
    {
        if (lookAtTarget)
        {
            originalLookPosition = unit.LookPosition;
            unit.SetLookPosition(target.transform.position);
        }

        OnDialogueStarted?.Invoke();
    }

    public void CompleteDialogue()
    {
        unit.SetLookPosition(originalLookPosition);
        OnDialogueCompleted?.Invoke();
    }
}
