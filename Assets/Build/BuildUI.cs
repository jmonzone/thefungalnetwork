using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
    [SerializeField] private BuildReference build;
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private Button closeButton;

    [SerializeField] private CameraPanController cameraPanController;

    private List<ItemUI> itemViewList = new List<ItemUI>();

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
            itemView.OnClick += () => build.StartBuild(itemView.Item);
        }

        var viewController = GetComponent<ViewController>();
        viewController.OnFadeInStart += ViewController_OnFadeInStart;
        viewController.OnFadeOutStart += ViewController_OnFadeOutStart;

        closeButton.onClick.AddListener(build.CancelBuild);
    }

    private void Start()
    {
        build.LoadExistingBuild();
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

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, interactableMask))
            {

                var buildController = hit.transform.GetComponentInParent<BuildController>();
                if (buildController)
                {
                    build.SelectBuild(buildController);
                    cameraPanController.CenterTargetInView(buildController.transform.position);
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
