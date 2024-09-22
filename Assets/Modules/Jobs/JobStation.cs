using System.Collections;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(ProximityAction))]
public abstract class JobStation : MonoBehaviour
{
    [Header("Job References")]
    [SerializeField] private InputManager playerController;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform playerPositionAnchor;
    [SerializeField] private Transform playerLookTarget;
    [SerializeField] private GameObject experienceContainer;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider experienceSlider;

    public FungalController Fungal { get; private set; }

    public event UnityAction OnJobStart;
    public event UnityAction OnJobEnd;

    protected virtual void Awake()
    {
        var proximityAction = GetComponent<ProximityAction>();
        proximityAction.OnUse += UseAction;

        enabled = false;
    }

    public void UseAction()
    {
        enabled = true;
        virtualCamera.Priority = 2;

        OnJobStarted();
        OnJobStart?.Invoke();

        StartCoroutine(WaitUntilCameraPrepared());
    }

    private IEnumerator WaitUntilCameraPrepared()
    {
        var mainCamera = Camera.main.transform;
        yield return new WaitUntil(() => mainCamera.position == virtualCamera.transform.position);
        OnCameraPrepared();
    }

    protected abstract void OnCameraPrepared();

    protected void EndAction()
    {
        enabled = false;
        virtualCamera.Priority = 0;

        OnJobEnded();
        OnJobEnd?.Invoke();
    }

    public void SetFungal(FungalController fungal)
    {
        Fungal = fungal;

        fungal.Model.OnExperienceChanged += _ => UpdateExperience();

        UpdateExperience();

        experienceContainer.SetActive(true);

        OnFungalChanged(fungal);
    }

    private void UpdateExperience()
    {
        levelText.text = Fungal.Model.Level.ToString();

        experienceSlider.minValue = FungalModel.ExperienceAtLevel(Fungal.Model.Level);
        experienceSlider.maxValue = FungalModel.ExperienceAtLevel(Fungal.Model.Level + 1);
        experienceSlider.value = Fungal.Model.Experience;
    }

    protected abstract void OnFungalChanged(FungalController fungal);
    protected abstract void OnJobStarted();
    protected abstract void OnJobEnded();
    protected abstract void OnBackButtonClicked();

}
