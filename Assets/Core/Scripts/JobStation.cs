using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public abstract class JobStation : EntityController
{
    [Header("Job References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CinemachineVirtualCamera camera;
    [SerializeField] private ControlPanel controlPanel;
    [SerializeField] private Sprite actionImage;
    [SerializeField] private Color actionColor;
    [SerializeField] private Transform playerPositionAnchor;
    [SerializeField] private Transform playerLookTarget;
    [SerializeField] private SlideAnimation uIAnimation;
    [SerializeField] private Button backButton;

    public override Sprite ActionImage => actionImage;
    public override Color ActionColor => actionColor;

    protected FungalController Fungal { get; private set; }
    public bool IsActive { get; private set; }

    private void Awake()
    {
        backButton.onClick.AddListener(() =>
        {
            OnBackButtonClicked();
        });

        uIAnimation.gameObject.SetActive(true);
    }


    public sealed override void UseAction()
    {
        IsActive = true;
        camera.Priority = 2;
        uIAnimation.IsVisible = true;
        controlPanel.SetVisible(false);

        Fungal = controlPanel.EscortedFungal;
        if (Fungal) controlPanel.UnescortFungal();

        playerController.Movement.SetPosition(playerPositionAnchor.position, () =>
        {
            playerController.Movement.SetLookTarget(playerLookTarget);
        });

        OnJobStarted();
    }

    protected abstract void OnJobStarted();

    protected abstract void OnBackButtonClicked();

    protected void EndAction()
    {
        IsActive = false;
        camera.Priority = 0;
        uIAnimation.IsVisible = false;
        StartCoroutine(ShowControlPanel());

        if (Fungal) Fungal.Stop();
    }

    private IEnumerator ShowControlPanel()
    {
        yield return new WaitForSeconds(1f);
        controlPanel.SetVisible(true);
    }
}
