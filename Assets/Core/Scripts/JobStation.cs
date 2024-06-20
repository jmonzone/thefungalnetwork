using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(ProximityAction))]
public abstract class JobStation : MonoBehaviour
{
    [Header("Job References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CinemachineVirtualCamera camera;
    [SerializeField] private ControlPanel controlPanel;
    [SerializeField] private Transform playerPositionAnchor;
    [SerializeField] private Transform playerLookTarget;
    [SerializeField] private SlideAnimation uIAnimation;
    [SerializeField] private Button backButton;

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

        var proximityAction = GetComponent<ProximityAction>();
        proximityAction.OnUse += UseAction;
    }

    public void UseAction()
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

        StartCoroutine(WaitUntilCameraPrepared());
    }

    private IEnumerator WaitUntilCameraPrepared()
    {
        var mainCamera = Camera.main.transform;
        yield return new WaitUntil(() => mainCamera.position == camera.transform.position);
        OnCameraPrepared();
    }

    protected abstract void OnCameraPrepared();

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
