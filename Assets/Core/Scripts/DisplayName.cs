using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class DisplayName : ScriptableObject
{
    [SerializeField] private LocalData localData;
    [SerializeField] private string value;

    public string Value => value;
    private const string DISPLAY_NAME_KEY = "displayName";

    public event UnityAction OnDisplayNameChanged;

    public void Initialize()
    {
        var name = localData.JsonFile[DISPLAY_NAME_KEY] ?? "";

        if (string.IsNullOrWhiteSpace(name.ToString()))
        {
            value = name.ToString();
        }
        else
        {
            value = name.ToString();
        }
    }

    public void SetValue(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            this.value = value;
            Debug.Log(value);
            localData.SaveData(DISPLAY_NAME_KEY, value);
            OnDisplayNameChanged?.Invoke();
        }
    }

    private string GenerateRandomName()
    {
        string[] adjectives = { "Party", "Quick", "Brat", "Lucid", "Fungal", "Bog" };
        string[] nouns = { "Exit", "Dragon", "Brat", "Wizard", "Warrior", "Fungal" };

        string adjective = adjectives[Random.Range(0, adjectives.Length)];
        string noun = nouns[Random.Range(0, nouns.Length)];
        int number = Random.Range(1000, 9999);

        return $"{adjective}{noun}{number}";
    }
}
