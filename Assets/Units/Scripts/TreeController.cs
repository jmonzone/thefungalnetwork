using Newtonsoft.Json.Linq;
using UnityEngine;

public class TreeController : UnitController
{
    [Header("Tree References")]
    [SerializeField] private TextAsset dialogueFile;
    [SerializeField] private Unit treeUnit;
    [SerializeField] private GlyphCollection glyphCollection;

    protected override void Awake()
    {
        base.Awake();

        string json = dialogueFile.text;
        JObject root = JObject.Parse(json);

        JObject data = (JObject)root["tree"];
        Dialogue.Initialize(data);

        var instance = ScriptableObject.CreateInstance<UnitInstance>();
        instance.Initialize(treeUnit);

        Initialize(instance);
    }
}
