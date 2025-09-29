using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DanceMoveUI : MonoBehaviour
{
    [SerializeField] private Image moveImage;
    [SerializeField] private TextMeshProUGUI moveNameText;
    [SerializeField] private Button button;

    private UnitDance dancer;

    private void Awake()
    {
        button.onClick.AddListener(UseMove);
    }

    public void SetMove(UnitDance dancer)
    {
        this.dancer = dancer;
    }

    private void UseMove()
    {
        dancer.PlayAnimation("Die02_SwordAndShield");
    }
}
