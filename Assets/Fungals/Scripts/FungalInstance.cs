using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public class FungalInstance : ScriptableObject
{
    [SerializeField] private Pet data;
    [SerializeField] private int index;
    [SerializeField] private float hunger;
    [SerializeField] private int level;
    [SerializeField] private float experience;

    public event UnityAction OnDataChanged;
    public event UnityAction<float> OnExperienceChanged;
    public event UnityAction<int> OnLevelChanged;
    public event UnityAction OnLevelUp;

    public int Index => index;
    public Pet Data => data;

    public JObject Json => new JObject
    {
        [ConfigKeys.NAME_KEY] = Data.name,
        [ConfigKeys.HUNGER_KEY] = Hunger,
        [ConfigKeys.LEVEL_KEY] = Level,
        [ConfigKeys.EXPERIENCE_KEY] = Experience
    };

    public void Initialize(Pet pet)
    {
        name = pet.Name;
        data = pet;
        level = 1;
        experience = 0;
        hunger = 100;
    }

    public void Initialize(int index, Pet pet, JObject json)
    {
        this.index = index;

        name = pet.Name;
        data = pet;
        level = (int)json[ConfigKeys.LEVEL_KEY];
        experience = (float)json[ConfigKeys.EXPERIENCE_KEY];
        hunger = (float)json[ConfigKeys.HUNGER_KEY];
    }

    public float Hunger
    {
        get => hunger;
        set
        {
            hunger = Mathf.Max(value, 0);
            OnDataChanged?.Invoke();
        }
    }

    public float Experience
    {
        get => experience;
        set
        {
            experience = value;
            OnExperienceChanged?.Invoke(experience);
            OnDataChanged?.Invoke();
            var requiredExperience = ExperienceAtLevel(level + 1);
            if (experience > requiredExperience) LevelUp();
        }
    }

    public int Level
    {
        get => level;
        set
        {
            level = value;
            OnLevelChanged?.Invoke(level);
            OnDataChanged?.Invoke();
        }
    }

    private void LevelUp()
    {
        Level++;
        OnLevelUp?.Invoke();
    }

    public static float ExperienceAtLevel(int level)
    {
        float total = 0;
        for (int i = 1; i < level; i++)
        {
            total += Mathf.Floor(i + 300 * Mathf.Pow(2, i / 7.0f));
        }

        return Mathf.Floor(total / 4);
    }
}