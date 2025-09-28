using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DancefloorReference : ScriptableObject
{
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference dancefloorIntro;
    [SerializeField] private ViewReference dancefloorGameplay;
    [SerializeField] private PlayerReference playerReference;

    [Header("Runtime")]
    [SerializeField] private Vector3 origin;
    [SerializeField] private List<UnitDance> units = new List<UnitDance>();

    public Vector3 Origin => origin;
    public List<UnitDance> Dancers => units;

    public event UnityAction OnDancefloorStart;
    public event UnityAction OnDancefloorExit;

    public void Initialize()
    {
        units = new List<UnitDance>();
    }

    public void StartDancefloor(List<UnitDance> units)
    {
        Debug.Log("Starting dancefloor");
        this.units = units;

        Vector3 sum = Vector3.zero;
        foreach (var unit in units)
        {
            sum += unit.transform.position;
        }

        origin = sum / units.Count;

        int count = units.Count;

        for (int i = 0; i < count; i++)
        {
            // Evenly spaced angle around circle, but clockwise
            float angle = -(i / (float)count) * Mathf.PI * 2f;

            // Direction from center (clockwise order)
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            // Position offset outward from center
            Vector3 destination = origin + direction * 1f;

            units[i].Unit.SetDestination(destination);
            units[i].Unit.SetLookPosition(origin);
        }

        navigation.Navigate(dancefloorGameplay);
        OnDancefloorStart?.Invoke();
    }

    public void ExitDancefloor()
    {
        OnDancefloorExit?.Invoke();
        units = new List<UnitDance>();
        navigation.GoBackToRoot();
    }
}
