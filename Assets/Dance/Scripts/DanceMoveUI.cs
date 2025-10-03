using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DanceMoveUI : MonoBehaviour
{
    [SerializeField] private Image moveImage;
    [SerializeField] private TextMeshProUGUI moveNameText;
    [SerializeField] private Button button;

    public void SetMove(DanceMoveInstance danceMove, UnityAction onClick)
    {
        moveNameText.text = danceMove.Label;
        moveImage.sprite = danceMove.Data.Sprite;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            onClick?.Invoke();
        });
    }
}
