using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuIntro : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private GameObject egg;
    [SerializeField] private FungalCollection fungalCollection;
    [SerializeField] private FungalInventory fungalInventory;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference homeReference;
    [SerializeField] private FadeCanvasGroup namePromptCanvas;

    private void Awake()
    {
        inputField.onValueChanged.AddListener(value => submitButton.interactable = value.Length > 2);

        submitButton.interactable = false;

        submitButton.onClick.AddListener(() =>
        {
            StartCoroutine(OnSubmitButtonClicked());
        });
    }

    private IEnumerator OnSubmitButtonClicked()
    {
        var randomIndex = Random.Range(0, fungalCollection.Fungals.Count);
        var randomFungal = fungalCollection.Fungals[randomIndex];
        var fungal = ScriptableObject.CreateInstance<FungalModel>();
        fungal.Initialize(randomFungal);
        fungalInventory.AddFungal(fungal);

        yield return namePromptCanvas.FadeOut();

        egg.SetActive(true);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        egg.SetActive(false);
        navigation.Navigate(homeReference);
    }
}
