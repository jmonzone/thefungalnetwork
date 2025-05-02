using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;

    private string[] loadingMessages = new string[]
    {
        "hey coach, get me out of this please",
        "the love never ends, is that good or bog?",
        "where the fungal is this so-called party grove?",
        "in the bog rooms we all fam",
        "when will you wake up and realize it's all bog",
        "oops here they are again, hiding in flesh",
        "careful, they feed off the poision",
        "don't tell me we need another seance",
        "it's not an ending, its the completion"
    };

    private void OnEnable()
    {
        if (loadingText != null && loadingMessages != null && loadingMessages.Length > 0)
        {
            int randomIndex = Random.Range(0, loadingMessages.Length);
            loadingText.text = loadingMessages[randomIndex];
        }
    }
}
