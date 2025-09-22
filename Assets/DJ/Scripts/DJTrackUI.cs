using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DJTrackUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DJTableReference dJTableReference;
    [SerializeField] private TextMeshProUGUI trackNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image trackImage;
    [SerializeField] private Button swapTrackButton;
    [SerializeField] private DJTrack track;

    [SerializeField] private int value;

    private void Awake()
    {
        swapTrackButton.onClick.AddListener(() =>
        {
            dJTableReference.RequestSwapTrack(value);
        });


    }

    private void OnEnable()
    {
        if (value == 0)
        {
            dJTableReference.OnLeftTrackChanged += DJTableReference_OnLeftTrackChanged;
            DJTableReference_OnLeftTrackChanged();
        }
        else
        {
            dJTableReference.OnRightTrackChanged += DJTableReference_OnRightTrackChanged;
            DJTableReference_OnRightTrackChanged();
        }
    }

    private void OnDisable()
    {
        if (value == 0) dJTableReference.OnLeftTrackChanged -= DJTableReference_OnLeftTrackChanged;
        else dJTableReference.OnRightTrackChanged -= DJTableReference_OnRightTrackChanged;
    }

    private void DJTableReference_OnLeftTrackChanged()
    {
        SetTrack(dJTableReference.LeftTrack);
    }
    private void DJTableReference_OnRightTrackChanged()
    {
        SetTrack(dJTableReference.RightTrack);
    }

    public void SetTrack(DJTrack track)
    {
        this.track = track;
        trackNameText.text = track.TrackName;
        descriptionText.text = track.Description;
        descriptionText.color = track.Glyph.Color;
        trackImage.sprite = track.Sprite;
    }
}
