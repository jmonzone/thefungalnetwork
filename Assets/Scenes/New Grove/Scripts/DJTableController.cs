using System.Collections;
using UnityEngine;

public class DJTableController : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private BackgroundMusicDelegate backgroundMusic;

    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;

    Transform IInteractable.Transform => transform;

    private void Awake()
    {
        var buildController = GetComponent<BuildController>();
        buildController.OnPlaced += BuildController_OnBuildComplete;
    }

    private void BuildController_OnBuildComplete()
    {
        StopAllCoroutines();
    }

    void IInteractable.OnSelect()
    {
        djReference.Show();
    }

    private float targetVolume1 = 1f; // default full
    private float targetVolume2 = 1f; // default full

    private Coroutine leftCoroutine;
    private Coroutine rightCoroutine;

    public void PlayLeftTrack(AudioClip audioClip, float targetVol = 1f)
    {
        targetVolume1 = targetVol;
        audioSource1.clip = audioClip;
        if (leftCoroutine != null) StopCoroutine(leftCoroutine);
        leftCoroutine = StartCoroutine(PlayAndFadeIn(audioSource1, targetVol, 5f));
    }

    public void PlayRightTrack(AudioClip audioClip, float targetVol = 1f)
    {
        targetVolume2 = targetVol;
        audioSource2.clip = audioClip;
        if (rightCoroutine != null) StopCoroutine(rightCoroutine);
        rightCoroutine = StartCoroutine(PlayAndFadeIn(audioSource2, targetVol, 5f));
    }

    public void SetSlider(float value)
    {
        // value between 0 and 1
        audioSource1.volume = (1f - value) * targetVolume1;
        audioSource2.volume = value * targetVolume2;
    }

    private IEnumerator PlayAndFadeIn(AudioSource source, float targetVolume, float duration)
    {
        source.volume = 0f;
        source.Play();

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, time / duration);
            yield return null;
        }
        source.volume = targetVolume;
    }

}
