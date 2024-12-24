using UnityEngine;
using UnityEngine.UI;

public class ModalUI : MonoBehaviour
{
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        var fadeCanvasGroup = GetComponent<FadeCanvasGroup>();
        continueButton.onClick.AddListener(() =>
        {
            StartCoroutine(fadeCanvasGroup.FadeOut());
        });
    }
}
