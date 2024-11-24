using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
        if (spellButton.PreviewItem && spellButton.PreviewItem.Data is ShruneItem shrune)
        {
            var direction = Input.mousePosition - mousePosition;
            StartCoroutine(ShootProjectile(direction, shrune.ProjectilePrefab));
        }
    }

    private IEnumerator ShootProjectile(Vector3 direction, Projectile projectilePrefab)
     {
        controller.SetAnimation();

        yield return new WaitForSeconds(0.5f);

        var projectile = Instantiate(projectilePrefab, controller.Movement.transform.position + Vector3.up, Quaternion.identity);
        direction.z = direction.y;
        direction.y = 0f;

        projectile.Shoot(direction);
    }


}
