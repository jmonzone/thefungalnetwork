using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitDJ : UnitBehaviour
{
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private List<DJTrack> playlist;

    private NavMeshAgent navMeshAgent;
    private int trackIndex;

    protected override void Awake()
    {
        base.Awake();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected override void OnBehaviourStart()
    {
        navMeshAgent.enabled = false;
        transform.position = djReference.Controller.DJPosition;

        // Start with first track on the Left
        trackIndex = 0;
        djReference.StartTrack(playlist[trackIndex]);

        // listen for when the left track ends
        djReference.Controller.OnLeftTrackComplete += OnLeftTrackEnding;
        djReference.Controller.OnRightTrackComplete += OnLeftTrackEnding;
    }

    private void OnLeftTrackEnding()
    {
        StartCoroutine(DJRoutine(isLeftToRight: true));
    }

    private void OnRightTrackEnding()
    {
        StartCoroutine(DJRoutine(isLeftToRight: false));
    }

    private IEnumerator DJRoutine(bool isLeftToRight)
    {
        // pick next track
        trackIndex = (trackIndex + 1) % playlist.Count;

        var startBPM = isLeftToRight ? djReference.LeftTrack.Bpm : djReference.RightTrack.Bpm;
        var endBPM = playlist[trackIndex].Bpm;

        if (isLeftToRight)
            djReference.SetRightTrack(playlist[trackIndex]);
        else
            djReference.SetLeftTrack(playlist[trackIndex]);

        var t = 0f;
        while (t < djReference.CrossFadeDuration)
        {
            t += Time.deltaTime;
            var interpolation = t / djReference.CrossFadeDuration;

            float fadeValue;
            if (interpolation <= 0.25f)
            {
                fadeValue = Mathf.Lerp(0f, 0.5f, interpolation / 0.25f);
            }
            else if (interpolation <= 0.75f)
            {
                fadeValue = 0.5f;
            }
            else
            {
                fadeValue = Mathf.Lerp(0.5f, 1f, (interpolation - 0.75f) / 0.25f);
            }

            // flip fade depending on direction
            djReference.SetTrackValue(isLeftToRight ? fadeValue : 1f - fadeValue);

            // BPM morph in middle half
            if (interpolation > 0.25f && interpolation < 0.75f)
            {
                var bpmInterp = (interpolation - 0.25f) / 0.5f;
                djReference.SetBPM(Mathf.Lerp(startBPM, endBPM, bpmInterp));
            }

            yield return null;
        }

        if (isLeftToRight) djReference.SetLeftTrack(null);
        else djReference.SetRightTrack(null);

        djReference.SetTrackValue(isLeftToRight ? 1f : 0f);
        djReference.SetBPM(endBPM);
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
        navMeshAgent.enabled = true;
        djReference.Controller.OnLeftTrackComplete -= OnLeftTrackEnding;
        djReference.Controller.OnRightTrackComplete -= OnRightTrackEnding;
    }
}
