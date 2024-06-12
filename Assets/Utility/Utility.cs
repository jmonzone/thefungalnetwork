using UnityEngine;

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
}
