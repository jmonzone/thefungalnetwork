using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference musicView;
    [SerializeField] private AudioSource audioSource;

    [Header("Beat Info")]
    [SerializeField] private int beat;   // current beat index
    private List<int> recordedBeats;     // spacebar hits

    private void Awake()
    {
        djReference.Initialize();
        recordedBeats = new List<int>();
    }

    private void OnEnable()
    {
        djReference.OnMusicStarted += DjReference_OnMusicStarted;
    }

    private void OnDisable()
    {
        djReference.OnMusicStarted -= DjReference_OnMusicStarted;
    }

    private void DjReference_OnMusicStarted()
    {
        audioSource = djReference.DjTable.AudioSource1;
        StartCoroutine(PlayToBeat());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Record the beat index when space is pressed
            recordedBeats.Add(beat);
            Debug.Log($"Space pressed on beat {beat}");

            // Example: if you want to move to the music view on first press
            // navigation.Navigate(musicView);
        }

        // Debug: press R to print all recorded beats
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Recorded beats: " + string.Join(", ", recordedBeats));
            recordedBeats = new List<int>();
        }
    }

    private IEnumerator PlayToBeat()
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
}
