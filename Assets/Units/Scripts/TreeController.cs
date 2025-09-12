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
        var treeId = "000000000000000000000001";
        var instance = new UnitInstance(treeId, treeUnit, 0, null);
        Initialize(instance);
    }
}
