using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class DJTable : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    private AudioSource audioSource;

    private Slider pitchSlider;

    [Header("Transition References")]
    [SerializeField] private GameObject inputCanvas;
    [SerializeField] private GameObject djCanvas;
    [SerializeField] private Button exitButton;

    private OverheadInteractionIndicator overheadInteraction;
    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();

        pitchSlider = GetComponentInChildren<Slider>();
        pitchSlider.onValueChanged.AddListener(value => audioSource.pitch = value);

        overheadInteraction = GetComponentInChildren<OverheadInteractionIndicator>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        var proximityAction = GetComponentInChildren<ProximityAction>();
        proximityAction.OnUse += Use;

        exitButton.onClick.AddListener(Exit);

        ToggleView(false);
    }

    private void Update()
    {
        float distance = Vector3.Distance(inputManager.Controllable.Movement.transform.position, transform.position);
        float maxDistance = 10f; // Adjust this value to control the range for volume falloff
        float minDistance = 3f;  // Range within which volume will be 1

        if (distance <= minDistance)
        {
            audioSource.volume = 1f;
        }
        else
        {
            float volume = Mathf.Clamp01(1 - Mathf.Log10(distance - minDistance + 1) / Mathf.Log10(maxDistance - minDistance + 1));
            audioSource.volume = volume;
        }
    }

    private void Use() => ToggleView(true);
    private void Exit() => ToggleView(false);

    private void ToggleView(bool value)
    {
        djCanvas.SetActive(value);
        inputCanvas.SetActive(!value);
        overheadInteraction.gameObject.SetActive(!value);
        virtualCamera.Priority = value ? 2 : 0;
    }
}
