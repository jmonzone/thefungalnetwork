using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DancefloorReference : ScriptableObject
{
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference dancefloorIntro;
    [SerializeField] private ViewReference dancefloorGameplay;
    [SerializeField] private PlayerReference playerReference;

    [Header("Debug")]
    [SerializeField] private bool skipIntro = false;
    [SerializeField] private List<UnitController> units = new List<UnitController>();

    public List<UnitController> Units => units;

    public event UnityAction OnDancefloorEnter;
    public event UnityAction OnDancefloorStart;
    public event UnityAction OnDancefloorExit;

    public void Initialize()
    {
        units = new List<UnitController>();
    }

    public void EnterDancefloor()
    {
        units = new List<UnitController>();
        units.Add(playerReference.Player);
        if (skipIntro)
        {
            StartDancefloor();
        }
        else
        {
            navigation.Navigate(dancefloorIntro);
            OnDancefloorEnter?.Invoke();
        }
    }

    public void StartDancefloor()
    {
        navigation.Navigate(dancefloorGameplay);
        OnDancefloorStart?.Invoke();
    }

    public void ExitDancefloor()
    {
        OnDancefloorExit?.Invoke();
        units.Remove(playerReference.Player);
        navigation.GoBackToRoot();
    }
}
