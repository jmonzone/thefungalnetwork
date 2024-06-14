using UnityEngine;

public class SlideAnimation : MonoBehaviour
{
    [SerializeField] private bool isVisible;
    [SerializeField] private Transform visibleAnchor;
    [SerializeField] private Transform hiddenAnchor;

    public bool IsVisible
    {
        get => isVisible;
        set => isVisible = value;
    }

    private void Awake()
    {
        if (isVisible) transform.position = visibleAnchor.transform.position;
        else transform.position = hiddenAnchor.transform.position;
    }

    private void Update()
    {
        var targetPosition = IsVisible ? visibleAnchor.position : hiddenAnchor.position;
        var direction = targetPosition - transform.position;

        if (direction.magnitude > 10f)
        {
            transform.position += 5f * Time.deltaTime * direction + direction.normalized;
        }
        else transform.position = targetPosition;
    }
}
