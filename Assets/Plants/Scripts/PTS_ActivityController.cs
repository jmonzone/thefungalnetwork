using System.Collections;
using UnityEngine;

public class PTS_ActivityController : ActivityController<PTS_Unit>
{
    [Header("References")]
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private PTS_SporeController sporeController;

    [Header("Runtime")]
    [SerializeField] private int passes;

    public PTS_SporeController SporeController => sporeController;

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
        if (!unit.IsPlayer) StartCoroutine(SporeRoutine());
    }

    private IEnumerator SporeRoutine()
    {
        yield return new WaitForSeconds(djReference.BeatDuration * 2);
        OnSporeComplete();
    }

    public void OnSporeComplete()
    {
        if (PlayerIsActive)
        {
            passes++;
            Debug.Log($"PTS_ActivityController passes: {passes}");
        }

        if (passes / Units.Count >= 1)
        {
            CurrentUnit.RemoveSpore(() =>
            {
                Activity.EndActivity();
            });
        }
        else
        {
            CurrentUnit.PassSpore(NextUnit, unit => SelectUnit(unit));
        }
    }

    protected virtual void Update()
    {
       
    }

    protected override void OnPlayerEnter(ActivityUnit player)
    {
        base.OnPlayerEnter(player);
        passes = -1;
    }
}
