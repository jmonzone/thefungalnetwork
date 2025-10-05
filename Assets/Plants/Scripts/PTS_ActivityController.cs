using System.Collections;
using UnityEngine;

public class PTS_ActivityController : ActivityController<PTS_Unit>
{
    [Header("References")]
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private PTS_SporeController sporeController;

    protected override void OnActivityStart()
    {
        base.OnActivityStart();
        sporeController.gameObject.SetActive(true);
    }

    protected override void OnActivityEnded()
    {
        base.OnActivityEnded();
        StopAllCoroutines();
        sporeController.gameObject.SetActive(false);
    }

    public override void SelectUnit(PTS_Unit unit)
    {
        base.SelectUnit(unit);
        CurrentUnit.GiveSpore(sporeController);
        if (!unit.IsPlayer) StartCoroutine(PassRoutine());
    }

    private IEnumerator PassRoutine()
    {
        yield return new WaitForSeconds(djReference.BeatDuration * 2);
        PassSpore();
    }

    public void PassSpore()
    {
        CurrentUnit.PassSpore(NextUnit, unit => SelectUnit(unit));
    }
}
