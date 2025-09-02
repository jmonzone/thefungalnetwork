using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class UnitForage : UnitBehaviour
{
    [Header("References")]
    [SerializeField] private SporeReference sporeReference;

    [Header("Runtime")]
    [SerializeField] private SporeController targetSpore;

    private NavMeshAgent agent;

    public SporeController TargetSpore => targetSpore;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        Unit.OnBehaviourChanged += UpdateTargets;
    }

    private void OnEnable()
    {
        sporeReference.OnSporeControllersChanged += UpdateTargets;
        UpdateTargets();
    }

    private void OnDisable()
    {
        sporeReference.OnSporeControllersChanged -= UpdateTargets;
    }

    private void UpdateTargets()
    {
        // Find closest spore
        targetSpore = null;
        foreach (var s in sporeReference.SporeControllers)
        {
            if (targetSpore == null || Vector3.Distance(transform.position, s.transform.position) < Vector3.Distance(transform.position, targetSpore.transform.position))
                targetSpore = s;
        }

        if (targetSpore)
        {
            if (Unit.IsDefaultBehaviour)
            {
                InvokeOnBehaviourRequest();
            }
        }
    }


    protected override void OnBehaviourStart()
    {
        if (targetSpore)
        {
            Debug.Log("OnBehaviourStart");
            agent.isStopped = false;
            StartCoroutine(ForagingBehaviour());
        }
    }

    private IEnumerator ForagingBehaviour()
    {
        while (targetSpore)
        {
            agent.SetDestination(targetSpore.transform.position);
            Unit.SetLookPosition(targetSpore.transform.position);
            yield return null;
        }

        StopBehaviour();
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
        agent.isStopped = true;
        StopAllCoroutines();
    }

}
