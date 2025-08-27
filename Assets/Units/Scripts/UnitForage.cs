using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class UnitForage : MonoBehaviour, IJob
{
    [Header("References")]
    [SerializeField] private SporeReference sporeReference;

    [Header("Runtime")]
    [SerializeField] private bool isAble;
    [SerializeField] private SporeController targetSpore;

    public bool IsAble => isAble;
    public Vector3 TargetPosition => targetSpore.transform.position;

    public event UnityAction OnIsAbleChanged;

    private void OnEnable()
    {
        sporeReference.OnSporeControllersChanged += FindClosestSpore;
    }

    private void OnDisable()
    {
        sporeReference.OnSporeControllersChanged -= FindClosestSpore;
    }

    private void FindClosestSpore()
    {
        if (sporeReference.SporeControllers.Count > 0)
        {
            targetSpore = sporeReference.SporeControllers[0];
            foreach (var spore in sporeReference.SporeControllers)
            {
                if (Vector3.Distance(transform.position, spore.transform.position) < Vector3.Distance(transform.position, targetSpore.transform.position))
                {
                    targetSpore = spore;
                }
            }
        }
        else targetSpore = null;

        isAble = targetSpore;
        OnIsAbleChanged?.Invoke();
    }

    private void Update()
    {
        if (targetSpore && Vector3.Distance(targetSpore.transform.position, transform.position) < 0.5f)
        {
            targetSpore.Collect();
        }
    }
}
