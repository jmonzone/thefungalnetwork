using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class PossesionService : ScriptableObject
{
    [SerializeField] private FungalService fungalService;
    [SerializeField] private FungalModel possessedFungal;

    public FungalModel PossessedFungal => possessedFungal;

    public event UnityAction OnPossessionChanged;
    private const string POSSESSION_KEY = "partner";

    public void Initialize(JObject jsonFile)
    {
        var posessionName = jsonFile[POSSESSION_KEY] ?? "";
        var posession = fungalService.Fungals.Find(fungal => fungal.Data.name == posessionName.ToString());
        SetPossession(posession);
    }

    public void SaveData(JObject jsonFile)
    {
        var possession = PossessedFungal?.Data.Id ?? "";
        jsonFile[POSSESSION_KEY] = possession;
    }

    public void SetPossession(FungalModel fungal)
    {
        possessedFungal = fungal;
        OnPossessionChanged?.Invoke();
    }
}
