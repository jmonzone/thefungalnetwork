using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class ResponseUI : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private FadeCanvasGroup continueGroup;
    [SerializeField] private FadeCanvasGroup responseGroup;
    [SerializeField] private List<ButtonUI> buttons = new List<ButtonUI>();

    public void ShowResponses(List<Response> responses, UnityAction<Response> onSelect)
    {
        // Assign responses to available options
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < responses.Count)
            {
                var response = responses[i];
                buttons[i].gameObject.SetActive(true);
                buttons[i].Initialize(response.Text, () => onSelect(response));
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        StartCoroutine(responseGroup.FadeIn());
    }

    public void ShowContinue(Action onClose)
    {
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => onClose());
        StartCoroutine(continueGroup.FadeIn());
    }

    public void Hide()
    {
        responseGroup.gameObject.SetActive(false);
        continueGroup.gameObject.SetActive(false);
    }
}
