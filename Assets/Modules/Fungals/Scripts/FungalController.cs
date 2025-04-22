using UnityEngine;
using UnityEngine.Events;

public class FungalController : MonoBehaviour
{
    [SerializeField] private FungalData data;

    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private GameObject shieldRenderer;
    [SerializeField] private GameObject trailRenderers;

    public ulong Id { get; set; }
    public FungalData Data => data;
    public float BaseSpeed => 3f;

    public Movement Movement { get; private set; }
    public Health Health { get; private set; }
    public MovementAnimations Animations { get; private set; }
    public MaterialFlasher MaterialFlasher { get; private set; }

    //todo: make separate death component
    public bool IsDead { get; private set; }

    public event UnityAction<GameObject, Vector3> OnRequestObjectSpawn;
    public event UnityAction<bool> OnShieldToggled;
    public event UnityAction<bool> OnTrailToggled;
    public event UnityAction<bool> OnDeath;
    public event UnityAction OnRespawnComplete;

    private void Awake()
    {
        Movement = GetComponent<Movement>();
        Movement.SetSpeed(baseSpeed);

        Health = GetComponent<Health>();

        OnRequestObjectSpawn += HandleObjectSpawn;

        if (data) InitializePrefab(data);
    }

    public void InitializePrefab(FungalData fungal)
    {
        //Debug.Log($"InitializePrefab {name} {fungal}");
        data = fungal;

        var model = Instantiate(data.Prefab, transform);
        Animations = model.AddComponent<MovementAnimations>();
        MaterialFlasher = model.AddComponent<MaterialFlasher>();
        MaterialFlasher.flashDuration = 0.5f;
    }

    public void Respawn()
    {
        IsDead = false;
        Movement.enabled = true;

        Animations.PlaySpawnAnimation();
        MaterialFlasher.FlashColor(Color.white);
        OnRespawnComplete?.Invoke();
    }

    public void Die(bool selfDestruct)
    {
        IsDead = true;
        Movement.enabled = false;

        Animations.PlayDeathAnimation();
        MaterialFlasher.FlashColor(Color.red);

        Movement.Stop();

        OnDeath?.Invoke(selfDestruct);
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

    public void SpawnObject(GameObject prefab, Vector3 position)
    {
        OnRequestObjectSpawn?.Invoke(prefab, position);
    }

    public void HandleObjectSpawn(GameObject prefab, Vector3 position)
    {
        Instantiate(prefab, position, Quaternion.identity);
    }

}
