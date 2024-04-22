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

    private enum HomeSceneState
    {
        EGG_SELECTION,
        PET_INFO
    }

    private void Start()
    {
        if (CurrentPet)
        {
            GoToPetInfo();
        }
        else
        {
            eggSelection.OnEggSelected += pet => StartCoroutine(OnEggSelected(pet));
            eggSelection.SetPets(Data.Pets.GetRange(0, 3));
            SetCurrentState(HomeSceneState.EGG_SELECTION);
        }

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
        SetCurrentState(HomeSceneState.PET_INFO);
    }

    private void SetCurrentState(HomeSceneState state)
    {
        eggSelection.gameObject.SetActive(state == HomeSceneState.EGG_SELECTION);
        petInfoManager.gameObject.SetActive(state == HomeSceneState.PET_INFO);
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
