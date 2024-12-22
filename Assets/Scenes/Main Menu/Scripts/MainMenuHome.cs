using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHome : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button partyButton;

    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference matchmakingView;

    private void Awake()
    {
        nameInputField.onValueChanged.AddListener(value =>
        {
            partyButton.interactable = value.Length > 3f;
        });

        partyButton.onClick.AddListener(() =>
        {
            navigation.Navigate(matchmakingView);
        });


    }
}
