using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConnectionUI : MonoBehaviour
{
    [SerializeField] private Button createButton;
    [SerializeField] private TMP_InputField lobbyCodeInput;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button exitButton;

    private bool CanJoin => lobbyCodeInput.text.Length == 6;

    public event UnityAction OnCreateButtonClicked;
    public event UnityAction<string> OnJoinButtonClicked;

    private void Awake()
    {
        exitButton.onClick.AddListener(() => SceneManager.LoadScene("Grove"));
        createButton.onClick.AddListener(() => OnCreateButtonClicked?.Invoke());
        joinButton.onClick.AddListener(() => OnJoinButtonClicked?.Invoke(lobbyCodeInput.text));

        joinButton.interactable = CanJoin;

        lobbyCodeInput.onValueChanged.AddListener(value =>
        {
            joinButton.interactable = CanJoin;
        });
    }
}
