using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIBehaviour : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        closeButton.onClick.AddListener(() => Hide());
    }

    public void Show() => SetVisible(true);

    public void Hide() => SetVisible(false);

    public void SetVisible(bool value)
    {
        canvasGroup.blocksRaycasts = value;

        if (value)
        {
            canvasGroup.alpha = 0;
            gameObject.SetActive(true);
            StartCoroutine(LerpAlpha(1));
        }
        else
        {
            StartCoroutine(LerpAlpha(0, () => gameObject.SetActive(false)));
        }
    }

    private IEnumerator LerpAlpha(float value, UnityAction onComplete = null)
    {
        var elapsedTime = 0f;
        var start = canvasGroup.alpha;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, value, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = value;
        onComplete?.Invoke();
    }
}
