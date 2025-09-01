using UnityEngine;
using UnityEngine.UI;

public class BuildPlacementUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BuildReference build;

    [Header("UI References")]
    [SerializeField] private Button buildButton;
    [SerializeField] private Button rotateLeftButton;
    [SerializeField] private Button rotateRightButton;
    [SerializeField] private Button cancelButton;

    private void Awake()
    {
        buildButton.onClick.AddListener(build.CompleteBuild);
        rotateLeftButton.onClick.AddListener(build.RotateLeft);
        rotateRightButton.onClick.AddListener(build.RotateRight);
        cancelButton.onClick.AddListener(build.CancelBuild);
    }

    private void Update()
    {
        buildButton.interactable = build.CurrentBuild && build.CurrentBuild.IsValidPlacement;
    }
}
