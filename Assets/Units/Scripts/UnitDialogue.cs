using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReference playerReference;

    [Header("Settings")]
    [SerializeField] private bool lookAtTarget = true;

    [Header("Runtime")]
    [SerializeField] private bool isActive = false;
    [SerializeField] private List<UnitController> targets;

    private UnitController unit;
    private Vector3 originalLookPosition;

    public bool IsActive => isActive;
    public List<UnitController> Targets => targets;

    public event UnityAction OnDialogueStarted;
    public event UnityAction OnDialogueCompleted;

    private void Awake()
    {
        unit = GetComponent<UnitController>();
        targets = new List<UnitController>();
    }

    public void StartDialogue(UnitController target)
    {
        isActive = true;
        targets.Add(target);

        if (lookAtTarget)
        {
            originalLookPosition = unit.LookPosition;
            unit.SetLookTarget(target.transform);
        }

        OnDialogueStarted?.Invoke();
    }

    public void StartGroupDialogue(Vector3 origin)
    {
        isActive = true;

        if (lookAtTarget)
        {
            originalLookPosition = unit.LookPosition;
            unit.SetLookPosition(origin);
        }

        OnDialogueStarted?.Invoke();
    }

    public void CompleteDialogue()
    {
        isActive = false;
        targets = new List<UnitController>();

        unit.SetLookPosition(originalLookPosition);
        OnDialogueCompleted?.Invoke();
    }
}
