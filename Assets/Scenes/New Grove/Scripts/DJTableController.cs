using System.Collections;
using UnityEngine;

public class DJTableController : InteractableController
{
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private BackgroundMusicDelegate backgroundMusic;

    private AudioSource audioSource;

    protected override UIReference Reference => djReference;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();

        var buildController = GetComponent<BuildController>();
        buildController.OnBuildComplete += BuildController_OnBuildComplete;
    }

    private void BuildController_OnBuildComplete()
    {
        StartCoroutine(PlayAndFadeIn(5f)); // 0.5 seconds fade
    }


    private IEnumerator PlayAndFadeIn(float duration)
    {
        backgroundMusic.HideMusic();

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
