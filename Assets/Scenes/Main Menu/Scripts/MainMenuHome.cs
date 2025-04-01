using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHome : MonoBehaviour
{
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference matchmakingView;

    [SerializeField] private DisplayName displayName;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI validationText;

    [SerializeField] private Button partyButton;

    private const int MINIMUM_NAME_LENGTH = 3;

    private void Awake()
    {
        nameInputField.onValueChanged.AddListener(value =>
        {
            validationText.gameObject.SetActive(value.Length <= MINIMUM_NAME_LENGTH);
            partyButton.interactable = value.Length > MINIMUM_NAME_LENGTH;
            displayName.SetValue(value);
        });

        partyButton.onClick.AddListener(() =>
        {
            navigation.Navigate(matchmakingView);
        });
    }

    private void OnEnable()
    {
        nameInputField.text = displayName.Value;
        partyButton.interactable = displayName.Value.Length > MINIMUM_NAME_LENGTH;
    }
}
