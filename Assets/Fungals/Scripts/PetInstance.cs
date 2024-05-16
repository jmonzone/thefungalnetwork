using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PetInstance : ScriptableObject
{
    [SerializeField] private Pet data;
    [SerializeField] private float hunger;

    public Pet Data => data;
    public float Hunger
    {
        get => hunger;
        set
        {
            hunger = Mathf.Max(value, 0);
            OnDataChanged?.Invoke();
        }
    }

    public event UnityAction OnDataChanged;

    public JObject Json => new JObject
    {
        ["name"] = Data.name,
        ["hunger"] = Hunger
    };

    public void Initialize(Pet pet)
    {
        data = pet;
        hunger = 100;
    }

    public void Initialize(Pet pet, JObject json)
    {
        data = pet;
        hunger = (float)json["hunger"];
    }
}