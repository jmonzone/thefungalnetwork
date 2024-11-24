using UnityEngine;
using UnityEngine.EventSystems;

public class SpellController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Controller controller;
    [SerializeField] private InventoryButton spellButton;

    private Vector3 mousePosition;

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        mousePosition = Input.mousePosition;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        var direction = Input.mousePosition - mousePosition;
        if (spellButton.PreviewItem && spellButton.PreviewItem.Data is ShruneItem shrune)
        {
            controller.SetAnimation();
            var spell = Instantiate(shrune.SpellPrefab, controller.Movement.transform.position + Vector3.up, Quaternion.identity);
            direction.z = direction.y;
            direction.y = 0f;

            spell.SetDirection(direction.normalized);
        }
    }


}
