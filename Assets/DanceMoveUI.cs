using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DanceMoveUI : MonoBehaviour
{
    [SerializeField] private Image moveImage;
    [SerializeField] private TextMeshProUGUI moveNameText;
    [SerializeField] private Button button;

    private UnitDance dancer;
    private string danceMove;

    private void Awake()
    {
        button.onClick.AddListener(UseMove);
    }

    public void SetMove(UnitDance dancer, string danceMove)
    {
        this.dancer = dancer;
        this.danceMove = danceMove;
    }

    private void UseMove()
    {
        dancer.PlayAnimation(danceMove);
    }
}
