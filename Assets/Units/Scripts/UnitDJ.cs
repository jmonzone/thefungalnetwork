using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitDJ : UnitBehaviour
{
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private List<DJTrack> tracks;

    private NavMeshAgent navMeshAgent;

    protected override void Awake()
    {
        base.Awake();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }


    protected override void OnBehaviourStart()
    {
        navMeshAgent.enabled = false;
        transform.position = djReference.DjTable.DJPosition;

        djReference.OnLeftTrackComplete += DjReference_OnLeftTrackComplete;
    }

    private void DjReference_OnLeftTrackComplete()
    {
        djReference.SwapTrack(tracks[0], 0);
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
        navMeshAgent.enabled = true;
        djReference.OnLeftTrackComplete -= DjReference_OnLeftTrackComplete;
    }
}
