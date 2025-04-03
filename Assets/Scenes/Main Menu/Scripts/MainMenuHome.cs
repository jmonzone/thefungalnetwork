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

    private void Start()
    {
        //Debug.Log($"MainMenuHome.Home {name}");

        nameInputField.onValueChanged.AddListener(value =>
        {
            var isValid = value.Length > MINIMUM_NAME_LENGTH;

            validationText.gameObject.SetActive(!isValid);
            partyButton.interactable = isValid;
            if (isValid) displayName.SetValue(value);
        });

        nameInputField.text = displayName.Value;

        partyButton.onClick.AddListener(() =>
        {
            navigation.Navigate(matchmakingView);
        });
    }

    private void OnEnable()
    {
        
    }
}
