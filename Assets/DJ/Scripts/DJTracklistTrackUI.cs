using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DJTracklistTrackUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DJTableReference djReference;

    [Header("UI References")]
    [SerializeField] private Image trackImage;
    [SerializeField] private TextMeshProUGUI trackNameText;
    [SerializeField] private TextMeshProUGUI artistText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button selectButton;

    private DJTrack track;

    private void Awake()
    {
        selectButton.onClick.AddListener(() =>
        {
            djReference.SwapTrack(track);
        });
    }

    public void SetTrack(DJTrack track)
    {
        this.track = track;
        trackImage.sprite = track.Sprite;
        trackNameText.text = track.name;
        artistText.text = track.Artist;
        descriptionText.text = track.Description;

        descriptionText.color = track.Glyph.Color;
    }
}
