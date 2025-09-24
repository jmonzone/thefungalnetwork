using System.Collections;
using UnityEngine;

public class DJController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference musicView;

    private void Awake()
    {
        djReference.Initialize();
    }

    private void Start()
    {
        StartCoroutine(PlayToBeat());
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            navigation.Navigate(musicView);
        }
    }

    private IEnumerator PlayToBeat()
    {
        int beat = 0;
        int maxBeats = 8;

        while (true)
        {
            djReference.InvokeBeat(beat);

            yield return new WaitForSeconds(djReference.BeatDuration);

            beat += 1;
            beat %= maxBeats;
        }
    }
}
