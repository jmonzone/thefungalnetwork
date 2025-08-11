using System.Collections;
using UnityEngine;

public class DJTableController : InteractableController
{
    [SerializeField] private CameraPanController cameraPanController;
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private GameObject render;
    [SerializeField] private DJTableReference djReference;

    private AudioSource audioSource;

    protected override UIReference Reference => djReference;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Inventory_OnItemSummoned();
    }

    private void OnEnable()
    {
        inventory.OnItemSummoned += Inventory_OnItemSummoned;
    }

    private void OnDisable()
    {
        inventory.OnItemSummoned += Inventory_OnItemSummoned;
    }

    private void Inventory_OnItemSummoned()
    {
        render.SetActive(inventory.HasDJTable);

        if (inventory.HasDJTable)
        {
            cameraPanController.CenterTargetInView(transform.position);
            StartCoroutine(PlayAndFadeIn(5f)); // 0.5 seconds fade
        }
    }

    private IEnumerator PlayAndFadeIn(float duration)
    {
        float targetVolume = 1; // Store the original volume
        audioSource.volume = 0f;
        audioSource.Play();

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, time / duration);
            yield return null;
        }
        audioSource.volume = targetVolume; // Ensure final volume is exact
    }
}
