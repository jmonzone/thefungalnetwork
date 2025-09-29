using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DanceMoveUI : MonoBehaviour
{
    [SerializeField] private Image moveImage;
    [SerializeField] private TextMeshProUGUI moveNameText;
    [SerializeField] private Button button;

    private UnitDance dancer;
    private DanceMove danceMove;

    private void Awake()
    {
        button.onClick.AddListener(UseMove);
    }

    public void SetMove(UnitDance dancer, DanceMove danceMove)
    {
        this.dancer = dancer;
        this.danceMove = danceMove;
        moveNameText.text = danceMove.Label;
        moveImage.sprite = danceMove.Sprite;
    }

    private void UseMove()
    {
        dancer.UseDanceMove(danceMove.AnimationName);
    }
}
