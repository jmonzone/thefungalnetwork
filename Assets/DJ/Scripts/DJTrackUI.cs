using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DJTrackUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DJTableReference dJTableReference;
    [SerializeField] private TextMeshProUGUI trackNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image trackImage;
    [SerializeField] private Button swapTrackButton;

    [SerializeField] private int value;

    private void Awake()
    {
        swapTrackButton.onClick.AddListener(() =>
        {
            dJTableReference.RequestSwapTrack(value);
        });
    }

    public void SetTrack(DJTrack track)
    {
        trackNameText.text = track.TrackName;
        descriptionText.text = track.Description;
        descriptionText.color = track.Glyph.Color;
        trackImage.sprite = track.Sprite;
    }
}
