using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private Image speakerImage;

    public void SetSpeaker(Unit data)
    {
        speakerText.text = data.Name;
        speakerImage.sprite = data.Sprite;
    }
}
