using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShruneTable : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject prefab;
    [SerializeField] private LayerMask planePoint;

    private GameObject item;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void SpawnItem()
    {
        item = Instantiate(prefab);
    }

    public void DropItem()
    {
        item = null;
    }

    private void Update()
    {
        if (item)
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f))
            {
                item.transform.position = hit.point;
            }

            if (Input.GetMouseButtonUp(0))
            {
                DropItem();
            }
        }
    }
}
