using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class ActivityReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference activityView;

    [Header("Runtime")]
    [SerializeField] private Vector3 origin;
    [SerializeField] private List<UnitController> units = new List<UnitController>();

    public Vector3 Origin => origin;
    public List<UnitController> Units => units;

    public event UnityAction OnActivityHasStarted;
    public event UnityAction OnActivityHasEnded;

    public void StartActivity(List<UnitController> units)
    {
        Debug.Log($"Starting activity {name}");
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

            units[i].SetDestination(destination);
            units[i].SetLookPosition(origin);
        }

        navigation.Navigate(activityView);
        OnActivityHasStarted?.Invoke();
    }

    public void EndActivity()
    {
        OnActivityHasEnded?.Invoke();
        units = new List<UnitController>();
        navigation.GoBackToRoot();
    }
}