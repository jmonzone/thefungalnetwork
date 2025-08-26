using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class SporeReference : ScriptableObject
{
    [SerializeField] private List<SporeController> sporeControllers;

    public List<SporeController> SporeControllers => sporeControllers;
    public event UnityAction OnSporeControllersChanged;

    public void Initialize()
    {
        sporeControllers = new List<SporeController>();
    }

    public void RegisterSpore(SporeController sporeController)
    {
        sporeControllers.Add(sporeController);
        OnSporeControllersChanged?.Invoke();
    }

    public void RemoveSpore(SporeController sporeController)
    {
        sporeControllers.Remove(sporeController);
        OnSporeControllersChanged?.Invoke();
    }
}
