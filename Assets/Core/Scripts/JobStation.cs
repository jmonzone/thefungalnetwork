using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
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

    public bool IsActive { get; private set; }

    public event UnityAction OnJobStart;
    public event UnityAction OnJobEnd;

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

        OnJobStart?.Invoke();

        playerController.Movement.SetPosition(playerPositionAnchor.position, () =>
        {
            playerController.Movement.SetLookTarget(playerLookTarget);
        });

        OnJobStarted();
    }

    protected void EndAction()
    {
        IsActive = false;
        camera.Priority = 0;
        uIAnimation.IsVisible = false;

        IEnumerator ShowControlPanel()
        {
            yield return new WaitForSeconds(1f);
            controlPanel.SetVisible(true);
        }

        StartCoroutine(ShowControlPanel());

        OnJobEnd?.Invoke();

        OnJobEnded();
    }

    public abstract void SetFungal(FungalController fungal);
    protected abstract void OnJobStarted();
    protected abstract void OnJobEnded();
    protected abstract void OnBackButtonClicked();

}
