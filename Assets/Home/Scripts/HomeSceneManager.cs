using System.Collections;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    [SerializeField] private string pet;

    public string Pet => pet;

    private string SaveDataPath => $"{Application.persistentDataPath}/data.json";
    private JObject saveData;


    public void LoadData()
    {
        if (File.Exists(SaveDataPath))
        {
            var configFile = File.ReadAllText(SaveDataPath);
            saveData = JObject.Parse(configFile);

            if (saveData.ContainsKey("currentPet")) pet = saveData["currentPet"].ToString();
        }
        else saveData = new JObject();
    }

    private void SaveData(string key, string value)
    {
        saveData[key] = value;
        File.WriteAllText(SaveDataPath, saveData.ToString());
    }

    public void SetCurrentPet(string pet)
    {
        this.pet = pet;
        SaveData("currentPet", pet);
    }
}

public class HomeSceneManager : MonoBehaviour
{
    [SerializeField] private EggSelection eggSelection;
    [SerializeField] private PetInfoManager petInfoManager;
    [SerializeField] private GameData gameData;
    [SerializeField] private List<PetData> petCollection;

    private enum HomeSceneState
    {
        EGG_SELECTION,
        PET_INFO
    }

    private void Start()
    {
        gameData = new GameData();
        gameData.LoadData();

        if (string.IsNullOrEmpty(gameData.Pet))
        {
            eggSelection.OnEggSelected += pet => StartCoroutine(OnEggSelected(pet));
            eggSelection.SetPets(petCollection.GetRange(0, 3));
            SetCurrentState(HomeSceneState.EGG_SELECTION);
        }
        else
        {
            GoToPetInfo();
        }
        
    }

    private IEnumerator OnEggSelected(PetData pet)
    {
        gameData.SetCurrentPet(pet.Name);
        yield return new WaitForSeconds(1f);
        GoToPetInfo();
    }

    private void GoToPetInfo()
    {
        petInfoManager.SetPet(petCollection.Find(pet => pet.Name == gameData.Pet));
        SetCurrentState(HomeSceneState.PET_INFO);
    }

    private void SetCurrentState(HomeSceneState state)
    {
        eggSelection.gameObject.SetActive(state == HomeSceneState.EGG_SELECTION);
        petInfoManager.gameObject.SetActive(state == HomeSceneState.PET_INFO);
    }
}
