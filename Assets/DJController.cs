using System.Collections;
using UnityEngine;

public class DJController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DJTableReference djReference;

    private void Awake()
    {
        djReference.Initialize();
    }

    private void Start()
    {
        StartCoroutine(PlayToBeat());
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
