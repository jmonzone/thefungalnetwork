using UnityEngine;
using UnityEngine.Events;

public class FungalController : MonoBehaviour
{
    [SerializeField] private FungalData data;
    [SerializeField] private FungalCollection fungalCollection;

    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private GameObject shieldRenderer;
    [SerializeField] private GameObject trailRenderers;

    public FungalData Data => data;
    public float BaseSpeed => 3f;

    public Movement Movement { get; private set; }
    public Health Health { get; private set; }
    public MovementAnimations Animations { get; private set; }
    public MaterialFlasher MaterialFlasher { get; private set; }

    public event UnityAction<bool> OnShieldToggled;
    public event UnityAction<bool> OnTrailToggled;

    private void Awake()
    {
        Movement = GetComponent<Movement>();
        Movement.SetSpeed(baseSpeed);

        Health = GetComponent<Health>();
    }

    public void InitializePrefab(int fungal)
    {
        //Debug.Log($"InitializePrefab {name} {fungal}");
        data = fungalCollection.Fungals[fungal];

        var model = Instantiate(data.Prefab, transform);
        Animations = model.AddComponent<MovementAnimations>();
        MaterialFlasher = model.AddComponent<MaterialFlasher>();
        MaterialFlasher.flashDuration = 0.5f;
    }

    public void ToggleShieldRenderers(bool value)
    {
        shieldRenderer.SetActive(value);
        OnShieldToggled?.Invoke(value);
    }

    public void ToggleTrailRenderers(bool value)
    {
        trailRenderers.SetActive(value);
        OnTrailToggled?.Invoke(value);
    }
}
