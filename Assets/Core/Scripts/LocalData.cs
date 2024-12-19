using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class LocalData : ScriptableObject
{
    [SerializeField] private bool resetDataOnAwake;
    [SerializeField] private string saveDataPath;

    public JObject JsonFile { get; private set; }

    public event UnityAction OnReset;

    public void Initialize()
    {
        saveDataPath = $"{Application.persistentDataPath}/data.json";
        if (Application.isEditor) saveDataPath = $"{Application.persistentDataPath}/data-editor.json";

        if (Application.isEditor && resetDataOnAwake) ResetData();

        if (!File.Exists(saveDataPath)) JsonFile = new JObject();
        else
        {
            try
            {
                var configFile = File.ReadAllText(saveDataPath);
                JsonFile = JObject.Parse(configFile);
            }
            catch
            {
                JsonFile = new JObject();
            }
        }
    }

    public void SaveData(string key, JToken value)
    {
        JsonFile[key] = value;
        File.WriteAllText(saveDataPath, JsonFile.ToString());
    }

    public void ResetData()
    {
        JsonFile = new JObject();
        File.WriteAllText(saveDataPath, JsonFile.ToString());
        OnReset?.Invoke();
    }
}
