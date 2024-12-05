using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class Possession : ScriptableObject
{
    [SerializeField] private FungalInventory fungalService;
    [SerializeField] private FungalModel fungal;
    [SerializeField] private Controller controller;

    public FungalModel Fungal => fungal;

    public event UnityAction OnPossessionChanged;
    private const string POSSESSION_KEY = "partner";

    public void Initialize(JObject jsonFile)
    {
        var posessionName = jsonFile[POSSESSION_KEY] ?? "";
        var posession = fungalService.Fungals.Find(fungal => fungal.Data.name == posessionName.ToString());
        SetPossession(posession);

        controller.OnUpdate += () =>
        {
            var fungalController = controller.Controllable.GetComponent<FungalController>();
            SetPossession(fungalController ? fungalController.Model : null);
        };
    }

    public void SaveData(JObject jsonFile)
    {
        var possession = Fungal?.Data.Id ?? "";
        jsonFile[POSSESSION_KEY] = possession;
    }

    public void SetPossession(FungalModel fungal)
    {
        this.fungal = fungal;
        OnPossessionChanged?.Invoke();
    }
}
