using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utility
{
    public static Vector3 RandomXZVector
    {
        get
        {
            var randomPosition = (Vector3)Random.insideUnitCircle;
            randomPosition.z = randomPosition.y;
            randomPosition.y = 0;
            return randomPosition;
        }
    }

    public static bool IsPointerOverUI
    {
        get
        {
            PointerEventData eventData = new(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new();
            EventSystem.current.RaycastAll(eventData, raysastResults);

            for (int index = 0; index < raysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = raysastResults[index];
                var maskContainsLayer = (5 & (1 << curRaysastResult.gameObject.layer)) != 0;

                if (maskContainsLayer) return true;
            }

            return false;
        }
    }
}
