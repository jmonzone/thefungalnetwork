using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public interface INoteTarget : ITarget
{
    public int EmissionStep {get;}
    public void OnHit();
}

public class DJNoteManager : MonoBehaviour
{
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private DJNoteController notePrefab;
    [SerializeField] private int poolSize = 20;

    [SerializeField] private UnitManager unitManager;
    [SerializeField] private BuildReference buildReference;
    [SerializeField] private List<PlantSporeEmitter> plants;


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
        buildReference.OnBuildUpdated += BuildReference_OnBuildUpdated;
    }

    private void OnDisable()
    {
        djReference.OnBeat -= DjReference_OnBeat;
        buildReference.OnBuildUpdated -= BuildReference_OnBuildUpdated;
    }

    private void BuildReference_OnBuildUpdated()
    {
        plants = new List<PlantSporeEmitter>();
        foreach (var build in buildReference.BuildControllers)
        {
            var plant = build.GetComponent<PlantSporeEmitter>();
            if (plant) plants.Add(plant);
        }
    }

    private void DjReference_OnBeat(int beat)
    {
        if (!djReference.DjTable) return;

        var phaseOffset = Random.Range(0f, Mathf.PI * 2f);

        EmitNotesForTrack(
            djReference.LeftValue,
            djReference.LeftTrack,
            GetTargetsForTrack(djReference.LeftTrack),
            beat,
            phaseOffset);

        EmitNotesForTrack(
            djReference.RightValue,
            djReference.RightTrack,
            GetTargetsForTrack(djReference.RightTrack),
            beat,
            phaseOffset);
    }

    private IEnumerable<INoteTarget> GetTargetsForTrack(DJTrack track)
    {
        var targets = track.PartyMode switch
        {
            PartyMode.Alternating => unitManager.AllUnits.Cast<INoteTarget>(),
            _ => plants.Cast<INoteTarget>()
        };

        return targets.Where(target =>
        {
            var unit = target.Transform.GetComponent<UnitController>();
            return !unit || unit.Data.Name != "Frog";
        });
    }

    private void EmitNotesForTrack(
        float value,
        DJTrack track,
        IEnumerable<INoteTarget> targets,
        int beat,
        float phaseOffset)
    {
        if (value <= 0.1f) return;

        foreach (var target in targets)
        {
            if (beat % target.EmissionStep != 0) continue;

            var note = GetFromPool();
            note.gameObject.SetActive(true);
            note.transform.position = djReference.DjTable.transform.position;
            note.Initialize(target, track.NoteColor, phaseOffset, value);

            void OnReached()
            {
                note.OnDestinationReached -= OnReached;
                target.OnHit();
            }
            note.OnDestinationReached += OnReached;
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
