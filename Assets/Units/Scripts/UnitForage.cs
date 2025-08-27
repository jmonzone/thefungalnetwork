using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class UnitForage : MonoBehaviour, IJob
{
    [Header("References")]
    [SerializeField] private SporeReference sporeReference;
    [SerializeField] private BuildReference buildReference;
    [SerializeField] private Item targetItem;

    [Header("Runtime")]
    [SerializeField] private bool isAble;
    [SerializeField] private SporeController targetSpore;
    [SerializeField] private BuildController targetBuild;

    bool IJob.IsAble => isAble;
    bool IJob.IsMoving => true;
    Vector3 IJob.TargetPosition => targetSpore.transform.position;

    public event UnityAction OnIsAbleChanged;
    public event UnityAction OnIsMovingChanged;

    private void OnEnable()
    {
        sporeReference.OnSporeControllersChanged += FindClosestSpore;
        buildReference.OnBuildUpdated += BuildReference_OnBuildUpdated;
        BuildReference_OnBuildUpdated();
    }

    private void OnDisable()
    {
        sporeReference.OnSporeControllersChanged -= FindClosestSpore;
    }

    private void FindClosestSpore()
    {
        if (sporeReference.SporeControllers.Count > 0)
        {
            targetSpore = sporeReference.SporeControllers[0];
            foreach (var spore in sporeReference.SporeControllers)
            {
                if (Vector3.Distance(transform.position, spore.transform.position) < Vector3.Distance(transform.position, targetSpore.transform.position))
                {
                    targetSpore = spore;
                }
            }
        }
        else targetSpore = null;

        isAble = targetSpore;
        OnIsAbleChanged?.Invoke();
    }

    private void BuildReference_OnBuildUpdated()
    {
        var builds = buildReference.FindBuildControllersWhere(targetItem);
        if (builds.Count > 0)
        {
            targetBuild = builds[0];
            foreach (var build in builds)
            {
                if (Vector3.Distance(transform.position, build.transform.position) < Vector3.Distance(transform.position, targetBuild.transform.position))
                {
                    targetBuild = build;
                }
            }
        }
        else targetBuild = null;

        isAble = targetBuild;
        OnIsAbleChanged?.Invoke();
    }

    private void Update()
    {
        if (targetSpore && Vector3.Distance(targetSpore.transform.position, transform.position) < 0.5f)
        {
            targetSpore.Collect();
        }
    }
}
