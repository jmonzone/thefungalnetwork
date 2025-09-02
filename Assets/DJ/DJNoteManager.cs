using UnityEngine;
using System.Collections.Generic;

public class DJNoteManager : MonoBehaviour
{
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private DJNoteController notePrefab;
    [SerializeField] private int poolSize = 20;

    private Queue<DJNoteController> notePool = new Queue<DJNoteController>();

    private void Awake()
    {
        // Prepopulate pool
        for (int i = 0; i < poolSize; i++)
        {
            DJNoteController note = InstantiateNote();
            notePool.Enqueue(note);
        }
    }

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
        foreach (var plant in djReference.Plants)
        {
            if (djReference.DjTable && beat % plant.EmissionStep == 0)
            {
                var phaseOffset = Random.Range(0f, Mathf.PI * 2f);

                DJNoteController note = GetFromPool();
                note.gameObject.SetActive(true);
                note.transform.position = djReference.DjTable.transform.position;
                note.Initialize(plant, djReference.LeftTrack.NoteColor, phaseOffset);

                note = GetFromPool();
                note.gameObject.SetActive(true);
                note.transform.position = djReference.DjTable.transform.position;
                note.Initialize(plant, djReference.RightTrack.NoteColor, phaseOffset + Mathf.PI);
            }
        }
    }

    private DJNoteController GetFromPool()
    {
        if (notePool.Count > 0)
        {
            DJNoteController note = notePool.Dequeue();
            return note;
        }
        else
        {
            // Optional: expand pool if needed
            DJNoteController note = InstantiateNote();
            return note;
        }
    }


    private DJNoteController InstantiateNote()
    {
        DJNoteController note = Instantiate(notePrefab, transform);
        note.OnDestinationReached += () => ReturnToPool(note);
        note.gameObject.SetActive(false);
        return note;
    }

    public void ReturnToPool(DJNoteController note)
    {
        note.gameObject.SetActive(false);
        notePool.Enqueue(note);
    }
}
