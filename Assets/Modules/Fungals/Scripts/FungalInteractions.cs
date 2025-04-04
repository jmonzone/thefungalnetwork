using UnityEngine;

public class FungalInteractions : MonoBehaviour
{
    [SerializeField] private GameObject placeholder;
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private string animationTrigger = "Clicked";

    private Vector2 startPos;
    private bool isDragging = false;
    private Camera mainCamera;

    private Animator fungalAnimator;
    private Quaternion originalRotation;

    private void Awake()
    {
        placeholder.SetActive(false);
        originalRotation = transform.rotation;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsTouchingModel(Input.mousePosition))
            {
                startPos = Input.mousePosition;
                isDragging = true;
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector2 delta = (Vector2)Input.mousePosition - startPos;
            transform.Rotate(0, -delta.x * rotationSpeed * Time.deltaTime, 0);
            startPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging && IsTouchingModel(Input.mousePosition))
            {
                PlayAnimation();
            }
            isDragging = false;
        }
    }

    private bool IsTouchingModel(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform.IsChildOf(transform);
        }
        return false;
    }

    private void PlayAnimation()
    {
        if (fungalAnimator)
        {
            fungalAnimator.Play("Clicked");
        }
    }

    public void SetFungal(GameObject fungal)
    {
        transform.rotation = originalRotation;
        fungalAnimator = fungal.GetComponent<Animator>();
        fungalAnimator.speed = 0.5f;
    }
}
