using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;

    private string[] loadingMessages = new string[]
    {
        "hey coach, get me out of this hell",
        "a moment of heaven, a lifetime of pain",
        "the love never ends, is it good or bog?",
        "where is the so called party grove?",
        "in the bog rooms we all fam",
        "when will you wake up and realize it's all bog",
        "oops here they are again, hiding in flesh",
        "careful otherwise they feed of the poision",
        "roads to isolation and wrath are well worn",
        "the previous footsteps have all been villainized",
        "the only antidote is to spill blood and tears",
        "the mark of empathy is provided to those who walked",
        "don't tell me we need another seance",
        "what you seek already exists, hidden behind a door",
        "lost in focus, seeing each other, but not the truth",
        "no escape, the exit is gone, will freedom be drawn"
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
