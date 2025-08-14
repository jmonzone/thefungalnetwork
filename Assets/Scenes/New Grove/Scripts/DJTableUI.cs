using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DJTableUI : MonoBehaviour
{
    [SerializeField] private BuildSystem build;

    [SerializeField] private List<PartyLightController> partyLights;
    [SerializeField] private DJTableController dJTableController;

    [SerializeField] private DJTrack track1;
    [SerializeField] private DJTrack track2;
    [SerializeField] private Slider trackSlider;

    [SerializeField] private DJTrackUI leftTrack;
    [SerializeField] private DJTrackUI rightTrack;

    private Coroutine partyCoroutine;

    public float staggerTime = 0.25f; // delay between groups in alternating

    private void Awake()
    {
        //leftTrack.OnClick += () =>
        //{
        //    dJTableController.PlayLeftTrack(leftTrack.Track.AudioClip);
        //    StartPartyLights(leftTrack.Track);
        //};

        //rightTrack.OnClick += () =>
        //{
        //    dJTableController.PlayRightTrack(rightTrack.Track.AudioClip);
        //    StartPartyLights(rightTrack.Track);
        //};

        leftTrack.SetTrack(track1);
        rightTrack.SetTrack(track2);

        trackSlider.onValueChanged.AddListener(value =>
        {
            dJTableController.SetSlider(value);

            if (value > 0.5)
            {
                currentTrack = rightTrack.Track;
            }
            else
            {
                currentTrack = leftTrack.Track;
            }
        });
    }

    private void Start()
    {
        Build_OnBuildLoaded();
    }

    private void OnEnable()
    {
        build.OnBuildLoaded += Build_OnBuildLoaded;
        build.OnBuildUpdated += Build_OnBuildLoaded;
    }

    private void OnDisable()
    {
        build.OnBuildLoaded -= Build_OnBuildLoaded;
        build.OnBuildUpdated -= Build_OnBuildLoaded;
    }

    private void Build_OnBuildLoaded()
    {
        partyLights = FindObjectsOfType<PartyLightController>().ToList();
        dJTableController = FindObjectOfType<DJTableController>();

        if (dJTableController)
        {
            dJTableController.PlayLeftTrack(leftTrack.Track.AudioClip);
            dJTableController.PlayRightTrack(rightTrack.Track.AudioClip);
            StartPartyLights(leftTrack.Track);
        }

        for (int i = 0; i < partyLights.Count; i++)
        {
            if (i % 2 == 0)
            {
                partyLights[i].phaseOffset = 0f;      // group 1
                partyLights[i].rotationSpeed *= 1;
            }
            else
            {
                partyLights[i].phaseOffset = 0.5f;
                partyLights[i].rotationSpeed *= -1;
            }
        }
    }

    private void StartPartyLights(DJTrack track)
    {
        currentTrack = track;

        if (partyCoroutine != null) StopCoroutine(partyCoroutine);

        partyCoroutine = StartCoroutine(PartyLightsRoutine());
    }

    private DJTrack currentTrack;
    private IEnumerator PartyLightsRoutine()
    {

        float beatDuration = 60f / currentTrack.Bpm; // seconds per beat

        //dJTableController.PlayMusic(track.AudioClip);

        while (true)
        {
            switch (currentTrack.PartyMode)
            {
                case PartyMode.Regular:
                    foreach (var light in partyLights)
                        light.SetEnabled(true);

                    yield return new WaitForSeconds(beatDuration);

                    foreach (var light in partyLights)
                        light.SetEnabled(false);

                    yield return new WaitForSeconds(beatDuration);
                    break;

                case PartyMode.Alternating:
                    int groups = 2; // can change to 3 for thirds
                    for (int group = 0; group < groups; group++)
                    {
                        for (int i = 0; i < partyLights.Count; i++)
                        {
                            if (i % groups == group)
                                partyLights[i].SetEnabled(true);
                        }

                        yield return new WaitForSeconds(beatDuration / 2);

                        for (int i = 0; i < partyLights.Count; i++)
                        {
                            if (i % groups == group)
                                partyLights[i].SetEnabled(false);
                        }

                        yield return new WaitForSeconds(beatDuration / 2);
                    }
                    break;

                case PartyMode.Strobe:
                    foreach (var light in partyLights)
                        light.SetEnabled(!light.Enabled);

                    yield return new WaitForSeconds(beatDuration / 4f); // 4 strobes per beat
                    break;
            }
        }
    }
}
