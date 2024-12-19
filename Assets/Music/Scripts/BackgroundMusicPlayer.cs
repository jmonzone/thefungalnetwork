using System.Collections;
using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    [SerializeField] private BackgroundMusicDelegate backgroundMusicDelegate;
    [SerializeField] private SceneNavigation sceneNavigation;

    private AudioSource audioSource;

    private const float VOLUME_TRANSITION_DURATION = 1.5f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;
        StartCoroutine(FadeVolume(1f));
    }

    private void OnEnable()
    {
        backgroundMusicDelegate.OnMusicRequested += BackgroundMusicDelegate_OnMusicRequested;
    }

    private void BackgroundMusicDelegate_OnMusicRequested(AudioClip audioClip)
    {
        if (audioSource.clip == audioClip) return;
        StopAllCoroutines();
        StartCoroutine(TransitionMusic(audioClip));
    }

    private void OnDisable()
    {
        backgroundMusicDelegate.OnMusicRequested -= BackgroundMusicDelegate_OnMusicRequested;
    }

    private IEnumerator TransitionMusic(AudioClip audioClip)
    {
        yield return FadeVolume(0f);

        audioSource.clip = audioClip;
        audioSource.Play();

        yield return FadeVolume(1f);
    }

    private IEnumerator FadeVolume(float targetVolume)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0;

        while (elapsedTime < VOLUME_TRANSITION_DURATION)
        {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / VOLUME_TRANSITION_DURATION);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}
