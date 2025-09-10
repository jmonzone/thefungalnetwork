using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private UnitListReference unitListReference;
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
    [SerializeField] private InventoryReference inventory;

    [Header("Runtime")]
    [SerializeField] private Unit unit;

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

    public void SetUnit(Unit unit)
    {
        this.unit = unit;

        if (unit)
        {
            image.sprite = unit.Sprite;
            nameText.text = unit.name;
            backgroundImage.color = backgroundActiveColor;
        }
        else
        {
            backgroundImage.color = backgroundInactiveColor;
        }

        image.gameObject.SetActive(unit);
        labelsContainer.SetActive(unit);
        summonContainer.SetActive(!unit);
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
