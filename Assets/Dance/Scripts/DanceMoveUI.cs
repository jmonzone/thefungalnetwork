using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DanceMoveUI : MonoBehaviour
{
    [SerializeField] private Image moveImage;
    [SerializeField] private TextMeshProUGUI moveNameText;
    [SerializeField] private Button button;

    private UnitDance dancer;
    private DanceMove danceMove;

    public event UnityAction OnDanceMoveStart;
    public event UnityAction OnDanceMoveComplete;

    private void Awake()
    {
        button.onClick.AddListener(UseMove);
    }

    public void SetMove(UnitDance dancer, DanceMove danceMove)
    {
        Debug.Log("DanceMoveUI SetMove");

        this.dancer = dancer;
        this.danceMove = danceMove;
        moveNameText.text = danceMove.Label;
        moveImage.sprite = danceMove.Sprite;
    }

    private void UseMove()
    {
        Debug.Log("DanceMoveUI UseMove");
        dancer.UseDanceMove(danceMove, OnDanceMoveComplete);
        OnDanceMoveStart?.Invoke();
    }
}
