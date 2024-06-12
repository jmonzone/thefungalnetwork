using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GardenManager : MonoBehaviour
{
    [Header("Developer Options")]
    [SerializeField] private bool disableControlPanel;

    [Header("Gameplay References")]
    [SerializeField] private FungalManager fungalManager;

    [Header("UI References")]
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private Button resetButton;
    [SerializeField] private ControlPanel controlPanel;
    [SerializeField] private InventoryList inventoryUI;
    [SerializeField] private InventoryList feedUI;


    private List<JobStation> jobStations = new List<JobStation>();
    private List<ItemInstance> Inventory => GameManager.Instance.Inventory;

    private enum GameState
    {
        GAMEPLAY,
        PET_INFO,
    }

    private void Start()
    {
        resetButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ResetData();
            SceneManager.LoadScene(0);
        });

        void UpdateInventory()
        {
            inventoryUI.SetInventory(Inventory);
            feedUI.SetInventory(Inventory);
        }

        GameManager.Instance.OnInventoryChanged += UpdateInventory;
        UpdateInventory();

        controlPanel.OnEscortButtonClicked += OnEscortButtonClicked;
        controlPanel.OnFungalInteractionEnd += fungalManager.EndFungalTalk;

        jobStations = FindObjectsOfType<JobStation>().ToList();
        foreach(var station in jobStations)
        {
            station.OnJobStart += () => Station_OnJobStart(station);
            station.OnJobEnd += Station_OnJobEnd;
        }

        if (!disableControlPanel)
        {
            gameplayCanvas.SetActive(true);
            controlPanel.gameObject.SetActive(true);
        }
    }

    private void Station_OnJobEnd()
    {
        if (fungalManager.EscortedFungal) fungalManager.UnescortFungal();
    }

    private void Station_OnJobStart(JobStation station)
    {
        var fungal = fungalManager.EscortedFungal;
        if (fungal)
        {
            fungalManager.UnescortFungal();
            station.SetFungal(fungal);
        }
    }

    private void OnEscortButtonClicked()
    {
        if (fungalManager.EscortedFungal)
        {
            fungalManager.UnescortFungal();
        }
        else
        {
            fungalManager.EscortFungal();
        }
    }

    
}
