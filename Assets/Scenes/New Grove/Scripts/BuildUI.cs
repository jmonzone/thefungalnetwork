using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask collisionMask;

    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    [SerializeField] private BuildSystem build;
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference buildView;
    [SerializeField] private ViewReference removeView;
    [SerializeField] private Button buildButton;
    [SerializeField] private Button removeButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private CameraPanController cameraPanController;

    private List<ItemUI> itemViewList = new List<ItemUI>();

    private BuildController buildController;


    private Vector3 startInput;
    private bool isDragging;
    private Camera mainCamera;
    [SerializeField] private LayerMask interactableMask;


    private void Awake()
    {
        mainCamera = Camera.main;

        GetComponentsInChildren(true, itemViewList);

        foreach(var itemView in itemViewList)
        {
            itemView.OnClick += () => ItemView_OnClick(itemView.Item);
        }

        var viewController = GetComponent<ViewController>();
        viewController.OnFadeInStart += ViewController_OnFadeInStart;
        viewController.OnFadeOutStart += ViewController_OnFadeOutStart;

        buildButton.onClick.AddListener(() =>
        {
            if (buildController)
            {
                build.AddBuild(buildController.Item, buildController.transform.position);
                buildController.CompleteBuild();
                buildController = null;
                navigation.GoBack();
            }
        });

        removeButton.onClick.AddListener(() =>
        {
            if (buildController)
            {
                build.RemoveBuild(buildController.Item);
                Destroy(buildController.gameObject);
                buildController = null;
                navigation.GoBack();
            }
        });

        closeButton.onClick.AddListener(() =>
        {
            if (buildController)
            {
                Destroy(buildController.gameObject);
                buildController = null;
            }
        });
    }

    private void ItemView_OnClick(Item item)
    {
        buildController = Instantiate(item.ItemPrefab).GetComponent<BuildController>();
        buildController.Initialize(item);
        buildController.StartBuild(groundMask, collisionMask, validMaterial, invalidMaterial);

        navigation.Navigate(buildView);

    }

    private bool canSelectBuildings = false;

    private void ViewController_OnFadeInStart()
    {
        UpdateView();

        canSelectBuildings = true;
    }


    private void ViewController_OnFadeOutStart()
    {
        canSelectBuildings = false;
    }

    private void Update()
    {
        if (!canSelectBuildings) return;

        if (Input.GetMouseButtonDown(0))
        {
            startInput = Input.mousePosition;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            var inputDelta = Input.mousePosition - startInput;
            if (inputDelta.magnitude > 0.1f) isDragging = true;
        }

        if (Input.GetMouseButtonUp(0) && !isDragging)
        {
            Vector3 inputPos = Input.mousePosition;

            Ray ray = mainCamera.ScreenPointToRay(inputPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, interactableMask))
            {

                buildController = hit.transform.GetComponentInParent<BuildController>();
                if (buildController)
                {
                    cameraPanController.CenterTargetInView(buildController.transform.position);
                    navigation.Navigate(removeView);
                    return;
                }
            }
        }
    }


    private void UpdateView()
    {
        var i = 0;
        foreach (var itemView in itemViewList)
        {
            if (i < inventory.Items.Count)
            {
                var item = inventory.Items[i];
                itemView.SetItem(item);
            }
            else
            {
                itemView.SetItem(null);
            }

            i++;
        }
    }
}
