using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum PartyMode
{
    Regular,
    Alternating,
    Strobe
}

public class DJTableUI : MonoBehaviour
{
    [SerializeField] private BuildSystem build;

    [SerializeField] private List<PartyLightController> partyLights;

    public Button regularButton;
    public Button alternatingButton;
    public Button strobeButton;

    private Coroutine partyCoroutine;

    public float staggerTime = 0.25f; // delay between groups in alternating

    private PartyMode currentMode = PartyMode.Regular;

    private void Awake()
    {
        regularButton.onClick.AddListener(() => { currentMode = PartyMode.Regular; StartPartyLights(); });
        alternatingButton.onClick.AddListener(() => { currentMode = PartyMode.Alternating; StartPartyLights(); });
        strobeButton.onClick.AddListener(() => { currentMode = PartyMode.Strobe; StartPartyLights(); });
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
    }

    private void StartPartyLights()
    {
        if (partyCoroutine != null) StopCoroutine(partyCoroutine);

        partyCoroutine = StartCoroutine(PartyLightsRoutine(currentMode));
    }

    private IEnumerator PartyLightsRoutine(PartyMode mode)
    {
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

        while (true)
        {
            switch (mode)
            {
                case PartyMode.Regular:
                    foreach (var light in partyLights)
                    {
                        light.SetEnabled(true);
                    }
                    yield return new WaitForSeconds(0.5f);
                    foreach (var light in partyLights)
                    {
                        light.SetEnabled(false);
                    }
                    yield return new WaitForSeconds(0.5f);
                    break;

                case PartyMode.Alternating:
                    int groups = 2; // can change to 3 for thirds
                    for (int group = 0; group < groups; group++)
                    {
                        for (int i = 0; i < partyLights.Count; i++)
                        {
                            if (i % groups == group)
                            {
                                partyLights[i].SetEnabled(true);
                            }
                        }
                        yield return new WaitForSeconds(0.5f);

                        for (int i = 0; i < partyLights.Count; i++)
                        {
                            if (i % groups == group)
                            {
                                partyLights[i].SetEnabled(false);
                            }
                        }
                        yield return new WaitForSeconds(staggerTime);
                    }
                    break;

                case PartyMode.Strobe:
                    foreach (var light in partyLights)
                    {
                        light.SetEnabled(!light.Enabled);
                    }
                    yield return new WaitForSeconds(0.1f);
                    break;
            }
        }
    }

}
