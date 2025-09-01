using UnityEngine;

public class DJNoteManager : MonoBehaviour
{
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private DJNoteController notePrefab;

    private void OnEnable()
    {
        djReference.OnBeat += DjReference_OnBeat;
    }

    private void OnDisable()
    {
        djReference.OnBeat -= DjReference_OnBeat;
    }

    private void DjReference_OnBeat(int beat)
    {
        foreach(var plant in djReference.Plants)
        {
            if (djReference.DjTable && beat % plant.EmissionStep == 0)
            {
                var note = Instantiate(notePrefab);
                note.transform.position = djReference.DjTable.transform.position;
                note.Initialize(plant);
            }

        }
    }
}
