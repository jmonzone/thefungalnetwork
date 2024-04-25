using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeSceneManager : BaseSceneManager
{
    [Header("Scene References")]
    [SerializeField] private EggSelection eggSelection;
    [SerializeField] private PetInfoManager petInfoManager;
    [SerializeField] private Button petInfoButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button closeButton;

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
            SetCurrentState(GameState.GAMEPLAY);
        }
        else
        {
            eggSelection.OnEggSelected += pet => StartCoroutine(OnEggSelected(pet));
            eggSelection.SetPets(Data.Pets.GetRange(0, 3));
            SetCurrentState(GameState.EGG_SELECTION);
        }

        petInfoButton.onClick.AddListener(() => GoToPetInfo());
        closeButton.onClick.AddListener(() => SetCurrentState(GameState.GAMEPLAY));

        resetButton.onClick.AddListener(() =>
        {
            ResetData();
            SceneManager.LoadScene(0);
        });
    }

    private IEnumerator OnEggSelected(Pet pet)
    {
        CurrentPet = pet;
        yield return new WaitForSeconds(1f);
        GoToPetInfo();
    }

    private void GoToPetInfo()
    {
        petInfoManager.SetPet(CurrentPet);
        petInfoManager.SetLevel(Level);
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
