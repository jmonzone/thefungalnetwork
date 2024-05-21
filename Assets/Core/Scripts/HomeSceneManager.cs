using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeSceneManager : BaseSceneManager
{
    [Header("Scene References")]
    [SerializeField] private EggSelection eggSelection;
    [SerializeField] private PetInfoManager petInfoManager;
    [SerializeField] private Button resetButton;
    [SerializeField] private PetController petController;
    [SerializeField] private Rigidbody player;
    [SerializeField] private ControlPanel controlPanel;
    [SerializeField] private InventoryList inventoryUI;
    [SerializeField] private InventoryList feedUI;

    private enum GameState
    {
        EGG_SELECTION,
        GAMEPLAY,
        PET_INFO
    }

    private void Start()
    {
        if (CurrentPet)
        {
            SpawnPet();
            SetCurrentState(GameState.GAMEPLAY);
        }
        else
        {
            eggSelection.OnEggSelected += pet => StartCoroutine(OnEggSelected(pet));
            eggSelection.SetPets(Data.Pets.GetRange(0, 3));
            SetCurrentState(GameState.EGG_SELECTION);
        }

        resetButton.onClick.AddListener(() =>
        {
            ResetData();
            SceneManager.LoadScene(0);
        });

        void UpdateInventory()
        {
            inventoryUI.SetInventory(Inventory);
            feedUI.SetInventory(Inventory);
        }

        OnInventoryChanged += UpdateInventory;
        UpdateInventory();
    }

    protected override void Update()
    {
        base.Update();

        if (petController.PetInstance)
        {
            var distance = Vector3.Distance(player.transform.position, petController.transform.position);
            if (distance < 4f) controlPanel.SetPetInteractions(petController);
            else controlPanel.SetPetInteractions(null);
        }
        else controlPanel.SetPetInteractions(null);
    }

    private IEnumerator OnEggSelected(Pet petData)
    {
        CurrentPet = ScriptableObject.CreateInstance<PetInstance>();
        CurrentPet.Initialize(petData);
        yield return new WaitForSeconds(1f);
        SpawnPet();
        SetCurrentState(GameState.GAMEPLAY);
    }

    private void SpawnPet()
    {
        petInfoManager.SetPet(CurrentPet);
        petInfoManager.SetLevel(Level);
        petController.SetPet(CurrentPet);
    }

    private void GoToPetInfo()
    {
        SetCurrentState(GameState.PET_INFO);
    }

    private void SetCurrentState(GameState state)
    {
        eggSelection.gameObject.SetActive(state == GameState.EGG_SELECTION);
        petInfoManager.gameObject.SetActive(state == GameState.PET_INFO);
    }

    protected override void OnExperienceChanged(float experience)
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnLevelChanged(int level)
    {
        //throw new System.NotImplementedException();
    }
}
