using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{
    [SerializeField] private Tutorial tutorial;
    [SerializeField] private Vector3 axis;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private DisplayName displayName;
    [SerializeField] private GameObject prompt;
    [SerializeField] private MainMenuUI mainMenu;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (tutorial.IsCompleted)
        {
            mainMenu.gameObject.SetActive(true);
            prompt.SetActive(false);
        }
        else
        {
            submitButton.onClick.AddListener(() =>
            {
                displayName.SetValue(inputField.text);
                SceneManager.LoadScene(1);
            });

            submitButton.interactable = false;
            inputField.onValueChanged.AddListener(value => submitButton.interactable = value.Length > 2);

            prompt.SetActive(true);
            mainMenu.gameObject.SetActive(false);
        }

    }

    private void Update()
    {
        mainCamera.transform.Rotate(axis, rotationSpeed * Time.deltaTime);
    }
}
