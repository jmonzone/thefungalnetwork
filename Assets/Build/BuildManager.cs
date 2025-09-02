using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BuildReference build;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference buildListView;
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private LayerMask interactableMask;

    private Camera mainCamera;
    private Vector3 startInput;
    private bool isDragging;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        build.LoadExistingBuild();
    }

    private void Update()
    {
        if (navigation.CurrentView != buildListView) return;

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
}
