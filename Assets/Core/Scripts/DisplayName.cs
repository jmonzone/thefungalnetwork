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

        //Debug.Log($"DisplayName.Initialize {name}");
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
            //Debug.Log(value);
            localData.SaveData(DISPLAY_NAME_KEY, value);
            OnDisplayNameChanged?.Invoke();
        }
    }
}
