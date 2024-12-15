using Newtonsoft.Json.Linq;
using UnityEngine;

[CreateAssetMenu]
public class Tutorial : ScriptableObject
{
    [SerializeField] private LocalData localData;
    [SerializeField] private bool isCompleted;

    public bool IsCompleted => isCompleted;
    private const string TUTORIAL_KEY = "tutorialCompleted";

    public void Initialize()
    {
        if (localData.JsonFile.TryGetValue(TUTORIAL_KEY, out JToken tutorialValue))
        {
            isCompleted = tutorialValue.Type == JTokenType.Boolean && tutorialValue.ToObject<bool>();
        }
        else
        {
            isCompleted = false;
        }
    }

    public void SetIsCompletd(bool isCompleted)
    {
        this.isCompleted = isCompleted;
        localData.SaveData(TUTORIAL_KEY, isCompleted);
    }
}
