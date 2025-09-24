using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DancefloorReference : ScriptableObject
{
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference dancefloorIntro;
    [SerializeField] private ViewReference dancefloorGameplay;
    [SerializeField] private PlayerReference playerReference;

    public event UnityAction OnDancefloorEnter;
    public event UnityAction OnDancefloorStart;
    public event UnityAction OnDancefloorExit;

    public void EnterDancefloor()
    {
        navigation.Navigate(dancefloorIntro);
        OnDancefloorEnter?.Invoke();
    }

    public void StartDancefloor()
    {
        navigation.Navigate(dancefloorGameplay);
        OnDancefloorStart?.Invoke();
    }

    public void ExitDancefloor()
    {
        navigation.GoBackToRoot();
        OnDancefloorExit?.Invoke();
    }
}
