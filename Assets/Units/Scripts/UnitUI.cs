using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private UnitListReference unitListReference;
    [SerializeField] private InventoryReference inventory;

    [Header("UI References")]
    [SerializeField] private Image image;
    [SerializeField] private Button unitButton;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color backgroundActiveColor;
    [SerializeField] private Color backgroundInactiveColor;
    [SerializeField] private RotateObject rotateObject;

    [SerializeField] private GameObject labelsContainer;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private BuyButton summonButton;
    [SerializeField] private GameObject summonContainer;

    [Header("Runtime")]
    [SerializeField] private UnitInstance unit;

    private void Awake()
    {
        summonButton.SetPrice(125);
        summonButton.OnBuy += () =>
        {
            inventory.DecreaseSporeCount(125);
            var blackList = unitListReference.Units;
            var randomUnit = unitListReference.GetRandomUnit(blackList);
            unitListReference.SummonUnit(randomUnit);
        };

        unitButton.onClick.AddListener(() => unitListReference.SelectFungal(unit));
    }

    public void SetUnit(UnitInstance unit)
    {
        this.unit = unit;

        if (unit != null)
        {
            image.sprite = unit.Data.Sprite;
            nameText.text = unit.Data.Name;
            levelText.text = $"Level {unit.FriendshipLevel}";
            backgroundImage.color = backgroundActiveColor;
        }
        else
        {
            backgroundImage.color = backgroundInactiveColor;
        }

        image.gameObject.SetActive(unit != null);
        labelsContainer.SetActive(unit != null);
        summonContainer.SetActive(unit == null);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        rotateObject.enabled = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        rotateObject.enabled = false;
    }
}
