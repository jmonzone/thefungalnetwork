using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Utility
{
    public static bool Contains(this LayerMask layerMask, int layer)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }

    public static void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    }

    public static List<T> OverlapSphere<T>(this Transform transform, float radius, Predicate<T> predicate = null) where T : MonoBehaviour
        => Physics.OverlapSphere(transform.position, radius)
        .Select(collider => collider.GetComponentInParent<T>())
        .Where(entity => entity && (predicate == null || predicate(entity)))
        .OrderBy(entity => Vector3.Distance(entity.transform.position, transform.position))
        .ToList();

    public static Vector3 GetRandomXZPosition(this Collider collider)
    {
        var x = UnityEngine.Random.Range(collider.bounds.min.x, collider.bounds.max.x);
        var z = UnityEngine.Random.Range(collider.bounds.min.z, collider.bounds.max.z);
        return new Vector3(x, 0, z);
    }

    public static Vector3 RandomXZVector
    {
        get
        {
            var randomPosition = (Vector3)UnityEngine.Random.insideUnitCircle;
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

    public static IEnumerator LerpAlpha(this Image background, float target, UnityAction onComplete = null)
    {
        var startColor = background.color;
        var targetColor = startColor;
        targetColor.a = target;

        var i = 0f;
        while (i < 1)
        {
            background.color = Color.Lerp(startColor, targetColor, i);
            i += Time.deltaTime;
            yield return null;
        }

        onComplete?.Invoke();
    }

    public static List<T> LoadAssets<T>() where T : ScriptableObject
    {
        var assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }

        Debug.Log($"Loaded {assets.Count} {typeof(T).Name} assets");

        return assets;

    }
}
