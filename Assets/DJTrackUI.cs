using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DJTrackUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI trackNameText;
    [SerializeField] private TextMeshProUGUI trackTypeText;
    [SerializeField] private Button button;
    [SerializeField] private DJTrack track;

    public DJTrack Track => track;
    public event UnityAction OnClick;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            OnClick?.Invoke();
        });
    }

    public void SetTrack(DJTrack track)
    {
        this.track = track;
        trackNameText.text = track.TrackName;
        trackTypeText.text = track.TrackType;
    }
}
