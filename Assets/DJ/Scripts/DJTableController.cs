using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DJTableController : MonoBehaviour, IInteractable
{
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference djView;
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private Transform djAnchor;

    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;

    public AudioSource AudioSource1 => audioSource1;
    public AudioSource AudioSource2 => audioSource2;

    Transform ITarget.Transform => transform;
    public Vector3 DJPosition => djAnchor.position;

    public event UnityAction OnLeftTrackComplete;
    public event UnityAction OnRightTrackComplete;

    private Coroutine leftCoroutine;
    private Coroutine rightCoroutine;
    private int beat;

    private void Awake()
    {
        var buildController = GetComponent<BuildController>();
        buildController.OnPlaced += BuildController_OnBuildComplete;
    }

    private void BuildController_OnBuildComplete()
    {
        djReference.Initialize(this);
        StopAllCoroutines();
        StartCoroutine(InvokeBeat());
    }

    private IEnumerator InvokeBeat()
    {
        beat = 0;
        int maxBeats = 8;

        while (true)
        {
            djReference.InvokeBeat(beat);

            yield return new WaitForSeconds(djReference.BeatDuration);

            beat++;
            beat %= maxBeats;
        }
    }

    private void DjReference_OnBPMChanged()
    {
        var bpm = djReference.BPM;

        if (djReference.LeftTrack)
        {
            audioSource1.pitch = bpm / djReference.LeftTrack.Bpm;
        }

        if (djReference.RightTrack)
        {
            audioSource2.pitch = bpm / djReference.RightTrack.Bpm;
        }
    }

    private void PlayLeftTrack(DJTrack track)
    {
        if (leftCoroutine != null) StopCoroutine(leftCoroutine);

        if (track)
        {
            Debug.Log($"playing left clip {track.name}");
            audioSource1.clip = track.AudioClip;
            leftCoroutine = StartCoroutine(PlayAndFadeIn(0));
        }
        else
        {
            audioSource1.Stop();
        }
    }

    private void PlayRightTrack(DJTrack track)
    {
        if (rightCoroutine != null) StopCoroutine(rightCoroutine);

        if (track)
        {
            Debug.Log($"playing right clip {track.name}");
            audioSource2.clip = track.AudioClip;
            rightCoroutine = StartCoroutine(PlayAndFadeIn(1));
        }
        else
        {
            audioSource2.Stop();
        }
    }

    private IEnumerator PlayAndFadeIn(int index)
    {
        var source = index == 0 ? audioSource1 : audioSource2;
        source.volume = 0f;
        source.Play();

        var targetVolume = index == 0 ? djReference.LeftValue : djReference.RightValue;

        float time = 0f;
        while (time < djReference.CrossFadeDuration)
        {
            time += Time.deltaTime;

            targetVolume = index == 0 ? djReference.LeftValue : djReference.RightValue;
            source.volume = Mathf.Lerp(0f, targetVolume, time / djReference.CrossFadeDuration);
            yield return null;
        }

        source.volume = targetVolume;


        // Wait for the clip to end
        yield return new WaitForSeconds(source.clip.length - djReference.CrossFadeDuration * 1.5f);
        if (index == 0)
        {
            OnLeftTrackComplete?.Invoke();
        }
        else
        {
            OnRightTrackComplete?.Invoke();
        }
    }

    private void DjReference_OnTrackValueChanged()
    {
        audioSource1.volume = djReference.LeftValue;
        audioSource2.volume = djReference.RightValue;
    }

    void IInteractable.Select()
    {
        navigation.Navigate(djView);
    }

    void IInteractable.OnProximityChanged(bool value)
    {
    }

    private void OnEnable()
    {
        djReference.OnLeftTrackChanged += PlayLeftTrack;
        djReference.OnRightTrackChanged += PlayRightTrack;
        djReference.OnBPMChanged += DjReference_OnBPMChanged;
        djReference.OnTrackValueChanged += DjReference_OnTrackValueChanged;
    }

    private void OnDisable()
    {
        djReference.OnLeftTrackChanged -= PlayLeftTrack;
        djReference.OnRightTrackChanged -= PlayRightTrack;
        djReference.OnBPMChanged -= DjReference_OnBPMChanged;
        djReference.OnTrackValueChanged -= DjReference_OnTrackValueChanged;
    }
}
