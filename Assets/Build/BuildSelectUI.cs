using UnityEngine;
using UnityEngine.UI;

public class BuildSelectUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BuildReference build;

    [Header("UI References")]
    [SerializeField] private Button removeButton;
    [SerializeField] private Button rotateLeftButton;
    [SerializeField] private Button rotateRightButton;
    [SerializeField] private Button cancelButton;

    private void Awake()
    {
        removeButton.onClick.AddListener(build.RemoveBuild);
        rotateLeftButton.onClick.AddListener(build.RotateLeft);
        rotateRightButton.onClick.AddListener(build.RotateRight);
        cancelButton.onClick.AddListener(build.CancelSelect);
    }
}
