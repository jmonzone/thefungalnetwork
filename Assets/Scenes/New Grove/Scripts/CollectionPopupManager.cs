using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CollectionPopupManager : MonoBehaviour
{
    [SerializeField] private InventoryReference inventoryReference;

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
    }

    private void OnEnable()
    {
        inventoryReference.OnSporeCollected += InventoryReference_OnSporeCollected;
    }

    private void OnDisable()
    {
        inventoryReference.OnSporeCollected -= InventoryReference_OnSporeCollected;
    }

    private void InventoryReference_OnSporeCollected(SporeController spore)
    {
        if (popupPool.Count == 0)
        {
            Debug.LogWarning("No popups available in pool!");
            return;
        }

        CollectionPopupUI popup = popupPool.Dequeue();
        popup.PlayPopup(spore.transform.position, 1);

        StartCoroutine(ReturnToPoolAfterDuration(popup, popup.duration));
    }

    private IEnumerator ReturnToPoolAfterDuration(CollectionPopupUI popup, float duration)
    {
        yield return new WaitForSeconds(duration);
        popupPool.Enqueue(popup);
    }
}
