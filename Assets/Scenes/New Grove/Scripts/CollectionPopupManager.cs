using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CollectionPopupManager : MonoBehaviour
{
    public InventoryController inventoryController;
    public Canvas uiCanvas;
    public Camera mainCamera;
    public CollectionPopupUI popupPrefab;

    private const int maxPopups = 5;
    private readonly Queue<CollectionPopupUI> popupPool = new Queue<CollectionPopupUI>();

    private void Awake()
    {
        // Pre-instantiate pool
        for (int i = 0; i < maxPopups; i++)
        {
            CollectionPopupUI popup = Instantiate(popupPrefab, uiCanvas.transform);
            popup.Initialize(uiCanvas, mainCamera);
            popup.gameObject.SetActive(false);
            popupPool.Enqueue(popup);
        }

        inventoryController.OnCollect += InventoryController_OnCollect;
    }

    private void InventoryController_OnCollect(Forageable forageable)
    {
        if (popupPool.Count == 0)
        {
            Debug.LogWarning("No popups available in pool!");
            return;
        }

        CollectionPopupUI popup = popupPool.Dequeue();
        popup.PlayPopup(forageable.transform.position);

        StartCoroutine(ReturnToPoolAfterDuration(popup, popup.duration));
    }

    private IEnumerator ReturnToPoolAfterDuration(CollectionPopupUI popup, float duration)
    {
        yield return new WaitForSeconds(duration);
        popupPool.Enqueue(popup);
    }
}
