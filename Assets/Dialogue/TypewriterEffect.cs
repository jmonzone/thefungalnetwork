using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private float baseSpeed = 0.03f;
    [SerializeField] private float punctuationPause = 0.2f;

    public IEnumerator TypeRoutine(string fullText, System.Action onComplete)
    {
        textUI.text = "";

        foreach (char c in fullText)
        {
            textUI.text += c;

            float delay = baseSpeed;
            if (".,!?:;".Contains(c.ToString()))
                delay += punctuationPause;

            delay *= Random.Range(0.9f, 1.3f);
            yield return new WaitForSeconds(delay);
        }

        onComplete?.Invoke();
    }
}
