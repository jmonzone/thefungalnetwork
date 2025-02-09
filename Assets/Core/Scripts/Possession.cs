using UnityEngine;

[CreateAssetMenu]
public class Possession : ScriptableObject
{
    [SerializeField] private LocalData localData;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private FungalModel fungal;
    [SerializeField] private PlayerReference controller;

    public FungalModel Fungal => fungal;

    private const string POSSESSION_KEY = "partner";

    public void Initialize()
    {
        var posessionName = localData.JsonFile[POSSESSION_KEY] ?? "";
        var posession = fungalInventory.Fungals.Find(fungal => fungal.Data.name == posessionName.ToString());
        if (!posession && fungalInventory.Fungals.Count > 0) posession = fungalInventory.Fungals[0];
        this.fungal = posession;

        //controller.OnUpdate += () =>
        //{
        //    //todo: centralize
        //    var networkFungal = controller.Movement.GetComponent<NetworkFungal>();
        //    if (networkFungal)
        //    {
        //        SetPossession(networkFungal.Fungal);
        //    }
        //    else
        //    {
        //        var fungalController = controller.Movement.GetComponent<FungalController>();
        //        SetPossession(fungalController ? fungalController.Model : null);
        //    }
        //};
    }

    public void SetPossession(FungalModel fungal)
    {
        this.fungal = fungal;
        var possession = fungal?.Data.Id ?? "";
        localData.SaveData(POSSESSION_KEY, possession);
    }
}
