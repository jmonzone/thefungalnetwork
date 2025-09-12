using Newtonsoft.Json.Linq;
using UnityEngine;

public class TreeController : UnitController
{
    [Header("Tree References")]
    [SerializeField] private TextAsset dialogue;
    [SerializeField] private Unit treeUnit;

    protected override void Awake()
    {
        base.Awake();

        string json = dialogue.text;
        JObject root = JObject.Parse(json);

        JObject data = (JObject)root["tree"];
        treeUnit.Initialize(data);

        var instance = ScriptableObject.CreateInstance<UnitInstance>();
        instance.Initialize(treeUnit);

        Initialize(instance);
    }
}
