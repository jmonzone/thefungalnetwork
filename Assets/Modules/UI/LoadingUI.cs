using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;

    private string[] loadingMessages = new string[]
    {
        "hey coach, get me out of this please",
        "the love never ends, is it good or bog?",
        "where is this so called party grove?",
        "in the bog rooms we all fam",
        "when will you wake up and realize it's all bog",
        "oops here they are again, hiding in flesh",
        "careful, they feed off the poision",
        "the only antidote is to spill blood and tears",
        "don't tell me we need another seance",
        "lost in focus, seeing each other, but not the truth",
        "no escape, the exit is gone, freedom is a draw",
        "it's not an ending, its completion"
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
