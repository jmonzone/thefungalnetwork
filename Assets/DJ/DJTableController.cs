using System.Collections;
using UnityEngine;

public class DJTableController : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoryReference inventory;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private BackgroundMusicDelegate backgroundMusic;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference djView;

    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;

    Transform ITarget.Transform => transform;

    private void Awake()
    {
        var buildController = GetComponent<BuildController>();
        buildController.OnPlaced += BuildController_OnBuildComplete;
    }

    private void OnEnable()
    {
        djReference.OnLeftTrackChanged += DjReference_OnLeftTrackChanged;
        djReference.OnRightTrackChanged += DjReference_OnRightTrackChanged;
    }

    private void OnDisable()
    {
        djReference.OnLeftTrackChanged -= DjReference_OnLeftTrackChanged;
        djReference.OnRightTrackChanged -= DjReference_OnRightTrackChanged;
    }

    private void DjReference_OnLeftTrackChanged()
    {
        PlayLeftTrack(djReference.LeftTrack.AudioClip);
    }

    private void DjReference_OnRightTrackChanged()
    {
        PlayRightTrack(djReference.RightTrack.AudioClip);
    }

    private void Start()
    {
        backgroundMusic.HideMusic();
        PlayLeftTrack(djReference.LeftTrack.AudioClip);
        PlayRightTrack(djReference.RightTrack.AudioClip);
    }

    private void BuildController_OnBuildComplete()
    {
        StopAllCoroutines();
        djReference.SetDJTable(this);
    }

    void IInteractable.Select()
    {
        navigation.Navigate(djView);
    }

    private Coroutine leftCoroutine;
    private Coroutine rightCoroutine;

    public void PlayLeftTrack(AudioClip audioClip)
    {
        audioSource1.clip = audioClip;
        if (leftCoroutine != null) StopCoroutine(leftCoroutine);
        leftCoroutine = StartCoroutine(PlayAndFadeIn(audioSource1, 1, 5f));
    }

    public void PlayRightTrack(AudioClip audioClip)
    {
        audioSource2.clip = audioClip;
        if (rightCoroutine != null) StopCoroutine(rightCoroutine);
        rightCoroutine = StartCoroutine(PlayAndFadeIn(audioSource2, 0, 5f));
    }

    public void SetSlider(float value)
    {
        // value between 0 and 1
        audioSource1.volume = (1f - value);
        audioSource2.volume = value;
    }

    public void SetLeftPitch(float value)
    {
        // value between 0 and 1
        audioSource1.pitch = value;
    }

    public void SetRightPitch(float value)
    {
        // value between 0 and 1
        audioSource2.pitch = value;
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
