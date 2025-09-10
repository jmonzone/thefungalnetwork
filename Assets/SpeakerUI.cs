using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private Image speakerImage;

    public void SetSpeaker(UnitInstance instance)
    {
        speakerText.text = instance.Data.Name;
        speakerImage.sprite = instance.Data.Sprite;
    }
}
