using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ResponseOption
{
    public Button button;
    public TextMeshProUGUI text;
}

public class ResponseUI : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private FadeCanvasGroup continueGroup;
    [SerializeField] private FadeCanvasGroup responseGroup;
    [SerializeField] private List<ResponseOption> options = new();

    public void ShowResponses(List<Response> responses, Action<Response> onSelect)
    {
        // Assign responses to available options
        for (int i = 0; i < options.Count; i++)
        {
            if (i < responses.Count)
            {
                SetupResponse(options[i], responses[i], onSelect);
                options[i].button.gameObject.SetActive(true);
            }
            else
            {
                options[i].button.gameObject.SetActive(false);
            }
        }

        StartCoroutine(responseGroup.FadeIn());
    }

    public void ShowContinue(System.Action onClose)
    {
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => onClose());
        StartCoroutine(continueGroup.FadeIn());
    }

    private void SetupResponse(ResponseOption option, Response response,
                               System.Action<Response> onSelect)
    {
        option.text.text = response.Text;
        option.button.onClick.RemoveAllListeners();
        option.button.onClick.AddListener(() =>
        {
            onSelect(response);
        });
    }

    public void Hide()
    {
        responseGroup.gameObject.SetActive(false);
        continueGroup.gameObject.SetActive(false);
    }
}
