using System.Collections;
using UnityEngine;

public class DJTableController : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private BackgroundMusicDelegate backgroundMusic;

    private AudioSource audioSource;

    Transform IInteractable.Transform => transform;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        var buildController = GetComponent<BuildController>();
        buildController.OnPlaced += BuildController_OnBuildComplete;
    }

    private void BuildController_OnBuildComplete()
    {
        StartCoroutine(PlayAndFadeIn(5f)); // 0.5 seconds fade
    }

    void IInteractable.OnSelect()
    {
        djReference.Show();
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
