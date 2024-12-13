using System.Linq;
using UnityEngine;

//public class ProximityInteraction : MonoBehaviour
//{
//    private const float MAXIMUM_PROXIMITY_DISTANCE = 3f;

//    public ProximityAction TargetAction { get; private set; }

//    private void Update()
//    {
//        var targetActions = Physics.OverlapSphere(transform.position, MAXIMUM_PROXIMITY_DISTANCE)
//           .Where(collider => !collider.isTrigger)
//           .Select(collider => collider.GetComponentInParent<ProximityAction>())
//           .Where(action => action && action.transform != transform)
//           .OrderBy(entity => Vector3.Distance(transform.position, entity.transform.position))
//           .ToList();

//        if (targetActions.Count > 0) TargetAction = targetActions.First();
//        else TargetAction = null;
//    }

//}
