using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public class FungalModel : ScriptableObject
{
    [SerializeField] private FungalData data;
    [SerializeField] private float hunger;
    [SerializeField] private int level;
    [SerializeField] private float experience;

    [Header("Fungal Stats")]
    [SerializeField] private float balance;
    [SerializeField] private float speed;
    [SerializeField] private float stamina;
    [SerializeField] private float power;

    public event UnityAction OnDataChanged;
    public event UnityAction<float> OnExperienceChanged;
    public event UnityAction<int> OnLevelChanged;
    public event UnityAction OnLevelUp;

    public FungalData Data => data;

    public JObject Json => new JObject
    {
        [ConfigKeys.NAME_KEY] = Data.name,
        [ConfigKeys.HUNGER_KEY] = Hunger,
        [ConfigKeys.LEVEL_KEY] = Level,
        [ConfigKeys.EXPERIENCE_KEY] = Experience,
        [ConfigKeys.STATS_KEY] = new JObject
        {
            [ConfigKeys.BALANCE_KEY] = balance,
            [ConfigKeys.SPEED_KEY] = speed,
            [ConfigKeys.STAMINA_KEY] = stamina,
            [ConfigKeys.POWER_KEY] = power,
        }
    };

    public void Initialize(FungalData pet)
    {
        name = pet.Id;
        data = pet;
        level = 1;
        experience = 0;
        hunger = 100;
        balance = 0;
        speed = 0;
        stamina = 0;
        power = 0;
    }

    public void Initialize(FungalData pet, JObject json)
    {
        name = pet.Id;
        data = pet;
        level = (int)json[ConfigKeys.LEVEL_KEY];
        experience = (float)json[ConfigKeys.EXPERIENCE_KEY];
        hunger = (float)json[ConfigKeys.HUNGER_KEY];
        balance = (float)json[ConfigKeys.STATS_KEY][ConfigKeys.BALANCE_KEY];
        speed = (float)json[ConfigKeys.STATS_KEY][ConfigKeys.SPEED_KEY];
        stamina = (float)json[ConfigKeys.STATS_KEY][ConfigKeys.STAMINA_KEY];
        power = (float)json[ConfigKeys.STATS_KEY][ConfigKeys.POWER_KEY];

    }

    #region Properties
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

    public float Balance
    {
        get => balance;
        set
        {
            balance = value;
            OnDataChanged?.Invoke();
        }
    }

    public float Speed
    {
        get => speed;
        set
        {
            speed = value;
            OnDataChanged?.Invoke();
        }
    }

    public float Stamina
    {
        get => stamina;
        set
        {
            stamina = value;
            OnDataChanged?.Invoke();
        }
    }

    public float Power
    {
        get => power;
        set
        {
            power = value;
            OnDataChanged?.Invoke();
        }
    }
    #endregion


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