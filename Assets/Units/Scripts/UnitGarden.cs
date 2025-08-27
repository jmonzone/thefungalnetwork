using UnityEngine;
using UnityEngine.Events;

public class UnitGarden : MonoBehaviour, IJob
{
    [Header("References")]
    [SerializeField] private BuildReference buildReference;
    [SerializeField] private Item targetItem;

    [Header("Runtime")]
    [SerializeField] private bool isAble;
    [SerializeField] private BuildController targetBuild;

    bool IJob.IsAble => isAble;

    Vector3 IJob.TargetPosition => targetBuild.transform.position;

    public event UnityAction OnIsAbleChanged;

    private void OnEnable()
    {
        buildReference.OnBuildUpdated += BuildReference_OnBuildUpdated;
        BuildReference_OnBuildUpdated();
    }

    private void OnDisable()
    {
        buildReference.OnBuildUpdated -= BuildReference_OnBuildUpdated;
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
}
